using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Sound", menuName = "FTG/Sound")]
public class Sound : ScriptableObject
{
    public AudioClip audioClip;
    public AudioMixerGroup audioGroup;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    [Range(0, 1f)] public float volumeVariation = 0f;
    [Range(0, 1f)] public float pitchVariation = 0f;
    public bool playOnAwake;
    public bool loop;

    [HideInInspector] public AudioSource source;
}
