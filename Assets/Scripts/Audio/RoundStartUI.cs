using System.Collections;
using UnityEngine;
using TMPro;

public class RoundStartUI : MonoBehaviour
{
    public GameObject blocker;
    public TMP_Text countdownText;
    public MonoBehaviour[] disableWhileCounting;

    public AudioSource backgroundMusic;
    public AudioSource sfxSource;
    public AudioClip countdownClip;
    public float sfxVolume = 1f;

    public float stepDelay = 1f;
    public float goHold = 1f;

    public Behaviour musicControllerComponent;
    public MusicController music;

    public CherryController cherry;
    public float requiredFirstSpawnFromSceneStart = 5f;

    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        // stop bgm during countdown
        if (backgroundMusic) { backgroundMusic.Stop(); backgroundMusic.playOnAwake = false; }
        if (musicControllerComponent) musicControllerComponent.enabled = false;

        // freeze controls + show blocker
        SetControls(false);
        if (blocker) blocker.SetActive(true);
        if (countdownText) countdownText.gameObject.SetActive(true);

        // 3-2-1-GO loop
        string[] steps = { "3", "2", "1", "GO!" };
        for (int i = 0; i < steps.Length; i++)
        {
            if (countdownText) countdownText.text = steps[i];
            if (sfxSource && countdownClip) sfxSource.PlayOneShot(countdownClip, sfxVolume);
            yield return new WaitForSeconds(i == steps.Length - 1 ? goHold : stepDelay);
        }

        // hide UI
        if (countdownText) countdownText.gameObject.SetActive(false);
        if (blocker) blocker.SetActive(false);

        // enable control + start bgm
        SetControls(true);
        if (musicControllerComponent) musicControllerComponent.enabled = true;
        if (music) music.PlayIntroThenNormal();
        else if (backgroundMusic)
        {
            backgroundMusic.loop = true;
            if (!backgroundMusic.isPlaying) backgroundMusic.Play();
        }

        // sync cherry spawn so it still happens at t≈5s
        float roundDuration = stepDelay * 3f + goHold;
        float extra = Mathf.Max(0f, requiredFirstSpawnFromSceneStart - roundDuration);
        if (cherry) cherry.Begin(extra);
    }

    void SetControls(bool enabled)
    {
        if (disableWhileCounting == null) return;
        for (int i = 0; i < disableWhileCounting.Length; i++)
            if (disableWhileCounting[i]) disableWhileCounting[i].enabled = enabled;
    }
}
