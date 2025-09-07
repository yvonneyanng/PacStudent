using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    
    public AudioSource source;
    public AudioClip introBGM;
    public AudioClip normalBGM;
    public AudioClip scaredBGM;
    public AudioClip ghostDeadBGM;
    
    void Awake()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        PlayIntroThenNormal();
    }

    void Update()
    {
        
    }
    
    public void PlayIntroThenNormal()
    {
        StopAllCoroutines();
        StartCoroutine(CoIntroThenLoop(normalBGM));
    }
    
    public void PlayScared() => Loop(scaredBGM);
    public void PlayGhostDead() => Loop(ghostDeadBGM);
    public void PlayNormal() => Loop(normalBGM);
    
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
