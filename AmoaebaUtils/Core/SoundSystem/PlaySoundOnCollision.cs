using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class PlaySoundOnCollision : ApplyOnCollision
{
    [SerializeField]
    private AudioClip[] sounds;

    [SerializeField]
    private BoolVar canPlaySound;

    [SerializeField]
    private string identifier;

    [SerializeField]
    private bool skipOnOverload = true;

    protected override void Apply(Transform Transform)
    {
        if(sounds.Length == 0 || (canPlaySound != null && !canPlaySound.Value))
        {
            return;
        }

        AudioClip sound = sounds[Random.Range(0, sounds.Length)];
        SoundSystem.Instance.PlaySound(sound, identifier, skipOnOverload);
    }
}
}