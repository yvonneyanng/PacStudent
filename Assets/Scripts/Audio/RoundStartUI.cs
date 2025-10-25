using System.Collections;
using UnityEngine;
using TMPro;

public class RoundStartUI : MonoBehaviour
{
    public GameObject blocker;
    public TMP_Text countdownText;
    public MonoBehaviour[] disableWhileCounting;
    public AudioSource backgroundMusic;
    public float stepDelay = 1f;
    public float goHold = 1f;
    public Behaviour musicControllerComponent;


    void Start(){ StartCoroutine(Run()); }

    IEnumerator Run()
    {
        SetControls(false);
        if (blocker) blocker.SetActive(true);
        if (countdownText) countdownText.gameObject.SetActive(true);

        if (countdownText) countdownText.text = "3";
        yield return new WaitForSeconds(stepDelay);
        if (countdownText) countdownText.text = "2";
        yield return new WaitForSeconds(stepDelay);
        if (countdownText) countdownText.text = "1";
        yield return new WaitForSeconds(stepDelay);
        if (countdownText) countdownText.text = "GO!";
        yield return new WaitForSeconds(goHold);

        if (countdownText) countdownText.gameObject.SetActive(false);
        if (blocker) blocker.SetActive(false);

        SetControls(true);
        if (musicControllerComponent) musicControllerComponent.enabled = true;


        if (backgroundMusic)
        {
            backgroundMusic.loop = true;
            if (!backgroundMusic.isPlaying) backgroundMusic.Play();
        }
    }

    void SetControls(bool enabled)
    {
        if (disableWhileCounting == null) return;
        for (int i = 0; i < disableWhileCounting.Length; i++)
            if (disableWhileCounting[i]) disableWhileCounting[i].enabled = enabled;
    }
}
