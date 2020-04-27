using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] string horizontalAxis = "FirstPersonHorizontalAxis";
    [SerializeField] KeyCode jumpingInput = KeyCode.JoystickButton0;
    [SerializeField] float minimumAxisValueToConsider = 0.25f;

    [Header("Collisions")]
    [SerializeField] BoxCollider boxCollider = default;
    [SerializeField] BoxRaycaster boxRaycaster = default;
    [SerializeField] float movementThreshold = 0.001f;

    private void Start()
    {
        jumpDurationSystem = new TimerSystem(jumpMaxDuration, EndJumping);
    }

    // Update is called once per frame
    void Update()
    {
        HandleActionsInputs();
        UpdateHorizontalMovementValues(Input.GetAxis(horizontalAxis));
        UpdateVerticalMovementValues();

        Vector3 movement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;
        Move(movement);
    }

    public void HandleActionsInputs()
    {
        if (Input.GetKeyDown(jumpingInput) && IsOnGround)
        {
            StartJumping();
        }
        else if(Input.GetKeyUp(jumpingInput) && isJumping)
        {
            EndJumping();
        }
    }

    #region Global Movement
    void Move(Vector3 movement)
    {
        Vector3 initialMovement = movement;

        if (movement.x != 0)
            movement.x = boxRaycaster.RaycastHorizontal(movement.x);
        if (Mathf.Abs(movement.x) < movementThreshold)
            movement.x = 0;

        if (movement.y != 0)
            movement.y = boxRaycaster.RaycastVertical(movement.y);
        if (Mathf.Abs(movement.y) < movementThreshold)
            movement.y = 0;

        if (movement.x > 0) boxRaycaster.flags.left = false;
        if (movement.x < 0) boxRaycaster.flags.right = false;
        if (movement.y > 0) boxRaycaster.flags.below = false;
        if (movement.y < 0) boxRaycaster.flags.above = false;

        if (initialMovement != movement)
        {
            if (boxRaycaster.flags.right && initialMovement.x > 0)
            {
                currentHorizontalSpeed = 0;
            }
            else if (boxRaycaster.flags.left && initialMovement.x < 0)
            {
                currentHorizontalSpeed = 0;
            }

            if (initialMovement.y < 0 && IsOnGround)
            {
                currentVerticalSpeed = 0;
            }

            if (initialMovement.y > 0 && boxRaycaster.flags.above)
            {
                currentVerticalSpeed = 0;
            }
        }

        

        transform.Translate(movement);
    }
    #endregion

    #region Horizontal Movement
    [Header("Horizontal Movement")]
    [SerializeField] float maxHorizontalSpeed = 5f;
    [SerializeField] float horizontalAcceleration = 20f;
    [SerializeField] float horizontalAccelerationTurningBack = 20f;
    [SerializeField] float horizontalDeceleration = 20f;
    [SerializeField] float airControl = 0.75f;
    [SerializeField] float airDrag = 0.1f;
    float currentHorizontalSpeed = 0f;

    public void UpdateHorizontalMovementValues(float input)
    {
        input = (Mathf.Abs(input) > minimumAxisValueToConsider) ? Mathf.Sign(input) : 0;
        if(input == 0)
        {
            currentHorizontalSpeed = Mathf.Clamp(
                currentHorizontalSpeed - Mathf.Sign(currentHorizontalSpeed) * horizontalDeceleration * Time.deltaTime * (IsOnGround ? 1 : airDrag), 
                currentHorizontalSpeed > 0 ? 0 : currentHorizontalSpeed, 
                currentHorizontalSpeed < 0 ? 0 : currentHorizontalSpeed );
        }
        else
        {
            float usedAcceleration = Mathf.Sign(currentHorizontalSpeed) == input ? horizontalAcceleration : horizontalAccelerationTurningBack;
            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed + usedAcceleration * Time.deltaTime * input * (IsOnGround ? 1 : airControl), 
                -maxHorizontalSpeed, maxHorizontalSpeed);
        }
        //print(currentHorizontalSpeed);
    }
    #endregion

    #region Jump and Gravity
    [Header("Jump and Gravity")]
    [SerializeField] float jumpForce = 20.0f;
    [SerializeField] float jumpMaxDuration = 0.3f;
    [SerializeField] float verticalGravity = -9.81f;
    [SerializeField] float maxVerticalDownVelocity = -20.0f;
    [SerializeField] float onGroundCheckDistance = 0.1f;

    float currentVerticalSpeed = 0f;
    TimerSystem jumpDurationSystem = new TimerSystem();
    bool isJumping = false;
    public bool IsOnGround => boxRaycaster.flags.below == true;

    public void UpdateVerticalMovementValues()
    {
        if (!isJumping)
        {
            if (IsOnGround)
                boxRaycaster.CheckForGroundBelow(onGroundCheckDistance);
            else
                currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + verticalGravity * Time.deltaTime, maxVerticalDownVelocity, currentVerticalSpeed);
        }
        else
        {
            jumpDurationSystem.UpdateTimer();
        }
    }

    public void StartJumping()
    {
        isJumping = true;
        currentVerticalSpeed = jumpForce;
        jumpDurationSystem.StartTimer();
        Debug.Log("JUMP");
    }

    public void EndJumping()
    {
        isJumping = false;
    }
    #endregion
}
