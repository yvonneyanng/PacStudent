using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacAudioDebug : MonoBehaviour
{
    public AudioSource sfx;
    public AudioClip moveClip;
    public AudioClip pelletClip;
    public AudioClip wallClip;
    public AudioClip deathClip;

    void Awake()
    {
        if (sfx == null) sfx = GetComponent<AudioSource>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) sfx.PlayOneShot(moveClip, 0.7f);
        if (Input.GetKeyDown(KeyCode.P)) sfx.PlayOneShot(pelletClip, 0.9f);
        if (Input.GetKeyDown(KeyCode.W)) sfx.PlayOneShot(wallClip, 0.8f);
        if (Input.GetKeyDown(KeyCode.K)) sfx.PlayOneShot(deathClip, 0.9f);
    }
}
