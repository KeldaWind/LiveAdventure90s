using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;



    [Header("Get Components")]
    public Life_UI lifeUI;
    public OutOfFrame_UI outOfFrameUI;
    public JetPackUI jetpackUI;
    public DamageableEntity herosLife;

    [Header("Lose Sight Condition")]
    public float maxOutOfFrameTime;

    public Animator winAnim;
    public Animator loseAnim;



    public void PlayWinAnim()
    {
        winAnim.Play("Win");
    }

    public void PlayLoseAnim()
    {
        loseAnim.Play("Lose");
    }

    private void Awake()
    {
        Instance = this;

        herosLife.OnDamageableEntitySetUp += lifeUI.SetLife;
        herosLife.OnLifeAmountChanged += lifeUI.RefreshLife;
    }
}
