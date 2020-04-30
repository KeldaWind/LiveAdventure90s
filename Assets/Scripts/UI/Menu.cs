using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Menu : MonoBehaviour
{
    [Header("")]
    public Animator pressAnim;
    private bool canPress = false;
    public PlayableAsset playable;

    private void Awake()
    {
        StartCoroutine(TimeBeforeTimelineEnd((float)playable.duration));
    }

    void Update()
    {
        if (canPress && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton6) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            LaunchTheGame();
        }
    }

    void LaunchTheGame()
    {
        canPress = false;
        pressAnim.Play("PressClick");
        SceneManager.LoadScene(1);
    }


    IEnumerator TimeBeforeTimelineEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        canPress = true;
    }
}
