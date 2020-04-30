using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Menu : MonoBehaviour
{
    [Header("Params")]
    public Animator pressAnim;
    private bool canPress = false;
    public PlayableAsset playable;
    public AudioSource music = default;

    private void Awake()
    {
        StartCoroutine(TimeBeforeTimelineEnd((float)playable.duration));
    }

    void Update()
    {
        if (canPress && (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Return))/*Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton6) || Input.GetKeyDown(KeyCode.UpArrow)*/)
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
        if (music)
            music.Play();

        canPress = true;
    }
}
