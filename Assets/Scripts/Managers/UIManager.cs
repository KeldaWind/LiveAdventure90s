using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public void ShowAllUI()
    {
        lifeUI.Show();
        outOfFrameUI.Show();
        jetpackUI.Show();
    }

    public void HideAllUI()
    {
        lifeUI.Hide();
        outOfFrameUI.Hide();
        jetpackUI.Hide();
    }

    [Header("Get Components")]
    public Life_UI lifeUI;
    public OutOfFrame_UI outOfFrameUI;
    public JetPackUI jetpackUI;
    public DamageableEntity herosLife;

    [Header("Lose Sight Condition")]
    public float maxOutOfFrameTime;

    [Header("End Animations")]
    public Animator winAnim;
    public Animator loseAnim;

    [Header("Pointer Animations")]
    public Animator pointerAnim;
    public Image pointerImage;
    private bool OnInteraction;

    private void Awake()
    {
        Instance = this;

        herosLife.OnDamageableEntitySetUp += lifeUI.SetLife;
        herosLife.OnLifeAmountChanged += lifeUI.RefreshLife;
    }


    public float GetLoseAnimationDuration()
    {
        return loseAnim.runtimeAnimatorController.animationClips[0].length;
    }

    public void PlayWinAnim()
    {
        winAnim.Play("Win");
        HideAllUI();
    }

    public void PlayLoseAnim()
    {
        loseAnim.Play("Lose");
        HideAllUI();
    }

    public void OnPointerInteraction()
    {
        if (OnInteraction)
            return;

        pointerImage.enabled = false;
        pointerAnim.Play("MovingObjectInRange");
        OnInteraction = true;
    }

    public void OnNormalInteraction()
    {
        if (!OnInteraction)
            return;

        pointerImage.enabled = true;
        pointerAnim.Play("None");
        OnInteraction = false;
    }
}
