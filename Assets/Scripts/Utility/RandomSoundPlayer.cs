using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private Vector2 cooldownRange = new(0.4f, 0.6f); 
    [SerializeField] private bool avoidOverlap = true; 
    private AudioSource scr;
    private float timer;

    private void Start()
    {
        scr = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    public void Play()
    {
        if (scr == null || clips == null || clips.Count < 1) return;
        if (avoidOverlap && scr.isPlaying) return;
        if (timer > 0f) return;

        int audioIndex = Random.Range(0, clips.Count);
        scr.clip = clips[audioIndex];
        scr.Play();

        timer = Random.Range(cooldownRange.x, cooldownRange.y);
    }
}
