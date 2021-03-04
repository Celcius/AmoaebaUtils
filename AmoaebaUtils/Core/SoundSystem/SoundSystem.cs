using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using AmoaebaUtils;
using System;

namespace AmoaebaUtils
{
public class SoundSystem : SingletonScriptableObject<SoundSystem>
{
    private struct SoundDefinition
    {
        public AudioClip clip;
        public string identifier;

        public Action<string> onFinishCallback;
        public AudioMixerGroup audioGroup;
    }

    private List<AudioSource> availableSources;
    private List<AudioSource> playingSources;
    private List<SoundDefinition> awaitingSlots;

    [SerializeField, Range(0, 1.0f)]
    private float mainVolume;
    public virtual float MainVolume
    {
        set  
        { 
            mainVolume = Mathf.Clamp01(value);
            foreach(AudioSource source in playingSources)
            {
                source.volume = mainVolume;
            }
        }

        get { return mainVolume; }
    }


    [SerializeField]
    private int maxConcurrentSounds = 12;

    protected virtual void OnEnable() 
    {
        availableSources = new List<AudioSource>();
        playingSources = new List<AudioSource>();
        awaitingSlots = new List<SoundDefinition>();
    }

    public void StopAllSounds()
    {
        awaitingSlots.Clear();

        foreach(AudioSource source in playingSources)
        {
            CoroutineRunner runner = source.GetComponent<CoroutineRunner>();
            Assert.IsFalse(runner == null, "Trying to stop sound from invalid source");
            runner.StopAllCoroutines();

            source.Stop();
            availableSources.Add(source);
        }

        playingSources.Clear();
    }

    public bool IsPlaying(string identifier)
    {
        foreach(AudioSource source in playingSources)
        {
            if(string.Compare(source.gameObject.name, identifier) == 0)
            {
                return true;
            }
        }
        return false;
    }

    public void StopSound(string identifier)
    {
        List<AudioSource> stoppedSources = new List<AudioSource>();
        foreach(AudioSource source in playingSources)
        {
            if(source.gameObject.name == identifier)
            {
                source.Stop();
                stoppedSources.Add(source);
            }
        }
        
        foreach(AudioSource source in stoppedSources)
        {
            OnPlayEnded(source);
        }
    }

    public void PlaySound(AudioClip clip, string identifier = "", bool skipOnOverload = true, AudioMixerGroup audioGroup = null, Action<string> onFinishCallback = null)
    {
        bool hasMaxSources = availableSources.Count + playingSources.Count >= maxConcurrentSounds;
        
        if(availableSources.Count == 0 && hasMaxSources)
        {
            if(!skipOnOverload)
            {
                SoundDefinition definition;
                definition.clip = clip;
                definition.identifier = identifier;
                definition.onFinishCallback = onFinishCallback;
                definition.audioGroup = audioGroup;
                awaitingSlots.Add(definition);
            }
            return;
        }

        if(availableSources.Count == 0 && !hasMaxSources)
        {
            int toCreate = maxConcurrentSounds - availableSources.Count - playingSources.Count;
            for(int i = 0; i < toCreate; i++)
            {
                CreateSoundSource();
            }
        }
        Play(clip, identifier, audioGroup, onFinishCallback);
    }

    private void Play(SoundDefinition definition)
    {
        Play(definition.clip, definition.identifier, definition.audioGroup, null);
    }

    private void Play(AudioClip clip, string identifier, AudioMixerGroup audioGroup, Action<string> onFinishCallback)
    {
        if(availableSources.Count <= 0)
        {
            return;
        }
        AudioSource source = availableSources[0];
        availableSources.Remove(availableSources[0]);
        playingSources.Add(source);
        source.gameObject.name = identifier;
        source.clip = clip;
        source.outputAudioMixerGroup = audioGroup;

        CoroutineRunner runner = source.GetComponent<CoroutineRunner>();

        Assert.IsFalse(runner == null, "Trying to play sound from incorrect source");
        
        source.volume = mainVolume;
        source.Play();
        runner.StartCoroutine(PlayRoutine(source, clip, identifier, onFinishCallback));   
    }

    private IEnumerator PlayRoutine(AudioSource source, AudioClip clip, string identifier, Action<string> onFinishCallback)
    {
        float clipLength = clip.length;
        yield return new WaitForSeconds(clipLength);
        OnPlayEnded(source);
        onFinishCallback?.Invoke(identifier);
    }

    private void OnPlayEnded(AudioSource source)
    {
        playingSources.Remove(source);
        availableSources.Add(source);
        source.name = "AvailableSoundSource";
        if(awaitingSlots.Count > 0)
        {
            SoundDefinition definition = awaitingSlots[0];
            awaitingSlots.Remove(definition);
            PlaySound(definition.clip, definition.identifier, false);
        }
    }
    
    private void CreateSoundSource()
    {
        CoroutineRunner runner = CoroutineRunner.Instantiate("SoundInstance");
        runner.name = "AvailableSoundSource";
        availableSources.Add(runner.gameObject.AddComponent<AudioSource>());
    }
}
}