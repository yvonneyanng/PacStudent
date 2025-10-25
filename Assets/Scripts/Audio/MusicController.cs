using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource source;
    public AudioClip introBGM;
    public AudioClip normalBGM;
    public AudioClip scaredBGM;
    public AudioClip ghostDeadBGM;

    bool isScaredPlaying = false;

    void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayIntroThenNormal();
    }

    void Update()
    {
        // check if GameManager exists and adjust based on scaredTimer
        if (GameManager.Instance)
        {
            bool shouldBeScared = GameManager.Instance.scaredTimer > 0f;
            if (shouldBeScared && !isScaredPlaying)
                PlayScared();
            else if (!shouldBeScared && isScaredPlaying)
                PlayNormal();
        }
    }

    public void PlayIntroThenNormal()
    {
        StopAllCoroutines();
        StartCoroutine(CoIntroThenLoop(normalBGM));
        isScaredPlaying = false;
    }

    public void PlayScared()
    {
        Loop(scaredBGM);
        isScaredPlaying = true;
    }

    public void PlayGhostDead()
    {
        Loop(ghostDeadBGM);
        isScaredPlaying = false;
    }

    public void PlayNormal()
    {
        Loop(normalBGM);
        isScaredPlaying = false;
    }

    IEnumerator CoIntroThenLoop(AudioClip loopClip)
    {
        if (introBGM != null)
        {
            source.loop = false;
            source.clip = introBGM;
            source.Play();
            yield return new WaitForSeconds(introBGM.length);
        }
        Loop(loopClip);
    }

    void Loop(AudioClip clip)
    {
        if (clip == null) return;
        source.loop = true;
        source.clip = clip;
        source.Play();
    }
}