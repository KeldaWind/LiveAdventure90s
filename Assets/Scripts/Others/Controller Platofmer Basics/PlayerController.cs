using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Movements")]
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float runMultiplier = 2;
    [SerializeField] float accelerationTime = 5;
    [SerializeField] AnimationCurve acceleration = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Vertical Movements")]
    [SerializeField] float gravity = 10;
    [SerializeField] AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float jumpReleaseMultipier = 1;
    [SerializeField] int maxJumpsAllowed = 1;

    [Header("References")]
    [SerializeField] Transform selfTr = default;
    [SerializeField] Transform graphicsTr = default;
    [SerializeField] SpriteRenderer sprRenderer = default;
    [SerializeField] Animator animator = default;
    [SerializeField] CharacterRaycaster raycaster = default;

    [Header("KeyBinding")]
    [SerializeField] KeyCode leftKey = KeyCode.LeftArrow;
    [SerializeField] KeyCode rightKey = KeyCode.RightArrow;
    [SerializeField] KeyCode jumpKey = KeyCode.UpArrow;
    [SerializeField] KeyCode actionKey = KeyCode.LeftShift;

    [Header("Debug")]
    [SerializeField] Transform fakeGroundHeigth = default;
    [SerializeField] bool debugMode = false;

    public bool isOnGround { get { return raycaster.flags.below; } }
    bool isJumping = false;
    int remainingJumpsCount = 0;
    Vector2 movementVector = Vector2.zero;
    float timeSinceJumped = 0;
    float timeSinceAccelerated = 0;
    float movementThreshold = 0.01f;

    private void Start()
    {
        timeSinceJumped = -10;
        //raycaster.OnReachedGround = Land;
    }

    private void Update()
    {
        InputUpdate();
        JumpUpdate();
        MovementUpdate();

        if (debugMode)
            DebugUpdate();

        AnimationUpdate();

        PostMovementJumpUpdate();
    }

    void InputUpdate()
    {
        movementVector = Vector2.zero;

        if (Input.GetKey(leftKey)) movementVector.x--;
        if (Input.GetKey(rightKey)) movementVector.x++;
        if (Input.GetKeyDown(jumpKey)) TryJump();
    }

    void JumpUpdate()
    {
        // pas défaut, la gravité s'applique vers le bas
        movementVector.y = gravity * -1f;

        // si on est pas en train de sauter, pas besoin de lire la suite
        if (!isJumping) return;

        // On vérifie si le joueur est en train d'appuyer sur saut ou non, pour prolonger ou raccourcir le saut
        float releaseMultiplier = Input.GetKey(jumpKey) ? 1 : jumpReleaseMultipier;
        // On actualise le temps depuis lequel on a commencé à sauter
        timeSinceJumped += Time.deltaTime * releaseMultiplier;
        // On récupère le multiplicatur de gravité actuel dans la courbe à l'aide du temps éoculé depuis le début du saut
        float gravityMultiplier = jumpCurve.Evaluate(timeSinceJumped);

        // On modifie la gravité à l'aide de ce multiplicateur
        movementVector.y *= gravityMultiplier * -1;

        // On arrête le saut si on est à la fin du mouvement
        if (timeSinceJumped > jumpCurve.keys[jumpCurve.keys.Length - 1].time)
            isJumping = false;
    }

    void PostMovementJumpUpdate()
    {
        if (isOnGround)
        {
            isJumping = false;
            remainingJumpsCount = maxJumpsAllowed;
        }
    }

    void MovementUpdate()
    {
        if (movementVector.x == 0) timeSinceAccelerated = 0;
        else timeSinceAccelerated += Time.deltaTime;

        float accelerationMultiplier = 0;
        if(accelerationTime > 0) accelerationMultiplier = acceleration.Evaluate(timeSinceAccelerated/ accelerationTime);
        float currentRunMultiplier  = Input.GetKey(actionKey) ? jumpReleaseMultipier : 1;

        float usedSpeed = maxSpeed * accelerationMultiplier * currentRunMultiplier;
        movementVector.x *= usedSpeed;

        Vector3 finalVector = Time.deltaTime * movementVector;
        Move(finalVector);
    }

    float side = 1;
    void AnimationUpdate()
    {
        if (movementVector.x != 0 && side != Mathf.Sign(movementVector.x))
        {
            side = Mathf.Sign(movementVector.x);
            Debug.Log("Change : " + movementVector.x);
        }

        Vector3 scale = graphicsTr.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * side;
        graphicsTr.transform.localScale = scale;
    }

    #region Jump
    void TryJump()
    {
        if (remainingJumpsCount == 0) return;

        remainingJumpsCount--;

        StartJump();
    }

    void StartJump()
    {
        isJumping = true;
        timeSinceJumped = 0;
    }

    void Land()
    {
        //isOnGround = true;
        remainingJumpsCount = maxJumpsAllowed;
    }
    #endregion

    #region Debug
    void DebugUpdate()
    {
        if(selfTr.position.y < fakeGroundHeigth.position.y)
        {
            //isOnGround = true;
            selfTr.position = new Vector3(selfTr.position.x, fakeGroundHeigth.position.y, selfTr.position.z);
        }
    }
    #endregion

    void Move(Vector3 movement)
    {
        if (movement.x != 0)
            movement.x = raycaster.RaycastHorizontal(movement.x);
        if (Mathf.Abs(movement.x) < movementThreshold)
            movement.x = 0;

        if (movement.y != 0)
            movement.y = raycaster.RaycastVertical(movement.y);
        if (Mathf.Abs(movement.y) < movementThreshold)
            movement.y = 0;

        if (movement.x > 0) raycaster.flags.left = false;
        if (movement.x < 0) raycaster.flags.right = false;
        if (movement.y > 0) raycaster.flags.below = false;
        if (movement.y < 0) raycaster.flags.above = false;

        selfTr.Translate(movement);
    }
}
