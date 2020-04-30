using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    [Header("Idle")]
    [SerializeField] float idleAmplitude = 0.5f;
    [SerializeField] float idleDuration = default;
    [SerializeField] AnimationCurve idleCurve = default;
    TimerSystem idleTimer = new TimerSystem();
    Vector3 idleStartPos = Vector3.zero;

    public void UpdateIdleMovement()
    {
        idleTimer.UpdateTimer();
        transform.position = idleStartPos + Vector3.up * idleCurve.Evaluate(idleTimer.GetTimerCounter) * idleAmplitude;

        if (idleTimer.TimerOver)
            idleTimer.StartTimer();
    }

    [Header("Loot")]
    [SerializeField] float lootDuration = 0.25f;
    [SerializeField] AnimationCurve lootCurve = AnimationCurve.Linear(0, 0, 1, 1);
    TimerSystem lootTimer = new TimerSystem();
    Vector3 lootStartLocation = Vector3.zero;

    [Header("Events")]
    public UnityEvent OnCollectEvent;

    ThirdPersonController targetCharacter = default;
    private void OnTriggerEnter(Collider other)
    {
        if (targetCharacter)
            return;

        targetCharacter = other.GetComponent<ThirdPersonController>();
        if (targetCharacter)
        {
            StartLootMovement();
        }
    }

    public void StartLootMovement()
    {
        lootTimer.StartTimer();
        lootStartLocation = transform.position;
        PlayStartLootFeedback();
    }

    public void UpdateLootMovement()
    {
        lootTimer.UpdateTimer();

        if (!lootTimer.TimerOver)
            transform.position = lootStartLocation + ((targetCharacter.transform.position + Vector3.up) - lootStartLocation) * lootCurve.Evaluate(lootTimer.GetTimerCoefficient);
    }

    public void EndMovement()
    {
        LootObject();
    }

    public void LootObject()
    {
        OnCollectEvent?.Invoke();
        PlayLootFeedback();
        Destroy(gameObject);
    }

    [Header("Feedbacks")]
    [SerializeField] string startLootFxTag = "PlaceHolder";
    [SerializeField] string lootFxTag = "PlaceHolder";
    [SerializeField] AudioManager.Sound lootSound = AudioManager.Sound.H_HeroRecupCollectible;

    public void PlayStartLootFeedback()
    {
        // FEEDBACK : Play sound
        FxManager.Instance.PlayFx(startLootFxTag, transform.position, Quaternion.identity, Vector3.one * 0.25f);
    }

    public void PlayLootFeedback()
    {
        AudioManager.PlaySound(lootSound);
        FxManager.Instance.PlayFx(lootFxTag, transform.position, Quaternion.identity, Vector3.one * 0.5f);
    }

    private void Start()
    {
        idleStartPos = transform.position;

        lootTimer = new TimerSystem(lootDuration, EndMovement);
        idleTimer = new TimerSystem(idleDuration, null);
        idleTimer.StartTimer();
        idleTimer.SetTime(Random.Range(0f, 1f));
    }

    private void Update()
    {
        if (targetCharacter)
        {
            UpdateLootMovement();
        }
        else
        {
            UpdateIdleMovement();
        }
    }
}
