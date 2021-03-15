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
        public bool loop;
    }

    private List<AudioSource> availableSources;
    private List<AudioSource> playingSources;
    private List<SoundDefinition> awaitingSlots;

    private const string AvailableName = "AvailableSoundSource";

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

        for(int i = 0; i < playingSources.Count; i++)
        {
            AudioSource source = playingSources[i];
            CoroutineRunner runner = source.GetComponent<CoroutineRunner>();
            Assert.IsFalse(runner == null, "Trying to stop sound from invalid source");
            runner.StopAllCoroutines();

            source.Stop();
            AddSingleSource(ref source, ref availableSources);
        }

        playingSources.Clear();
    }

    public bool IsPlaying(string identifier)
    {
        foreach(AudioSource source in playingSources)
        {
            if(string.Compare(source.gameObject.name, identifier) == 0 && source.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public void StopSound(string identifier)
    {
        List<AudioSource> stoppedSources = new List<AudioSource>();
        for(int i = 0; i < playingSources.Count; i++)
        {
            AudioSource source = playingSources[i];
            if(source.gameObject.name == identifier)
            {
                source.Stop();
                AddSingleSource(ref source, ref stoppedSources);
            }
        }
        
        foreach(AudioSource source in stoppedSources)
        {
            OnPlayEnded(source);
        }
    }

    public void PlaySound(AudioClip clip, string identifier = "", bool skipOnOverload = true, AudioMixerGroup audioGroup = null, Action<string> onFinishCallback = null, bool loop = false)
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
                definition.loop = loop; 
                awaitingSlots.Add(definition);
            }
            return;
        }

        if(availableSources.Count == 0 && !hasMaxSources)
        {
            CreateSoundSource();
        }
        Play(clip, identifier, audioGroup, onFinishCallback, loop);
    }

    private void Play(SoundDefinition definition)
    {
        Play(definition.clip, definition.identifier, definition.audioGroup, null, definition.loop);
    }

    private void Play(AudioClip clip, string identifier, AudioMixerGroup audioGroup, Action<string> onFinishCallback, bool loop)
    {
        if(availableSources.Count <= 0)
        {
            return;
        }
        AudioSource source = availableSources[0];

        if(source.name != "AvailableSoundSource" || playingSources.Contains(source))
        {
            Debug.LogError("Potential override of soundSource " + source.name);
        }

        availableSources.Remove(availableSources[0]);
        

        AddSingleSource(ref source, ref playingSources);

        source.gameObject.name = identifier;
        source.clip = clip;
        source.outputAudioMixerGroup = audioGroup;
        source.loop = loop;

        CoroutineRunner runner = source.GetComponent<CoroutineRunner>();

        Assert.IsFalse(runner == null, "Trying to play sound from incorrect source");
        
        source.volume = mainVolume;
        source.Play();
        if(!loop)
        {
            runner.StartCoroutine(PlayRoutine(source, clip, identifier, onFinishCallback));   
        }
    }

    private IEnumerator PlayRoutine(AudioSource source, AudioClip clip, string identifier, Action<string> onFinishCallback)
    {
        if(clip == null)
        {
            Debug.LogError("NULL clip " + identifier + " in sound system for source " + source.name);
            OnPlayEnded(source);
            onFinishCallback?.Invoke(identifier);
            yield break;
        }
        float clipLength = clip.length;
        yield return new WaitForSeconds(clipLength);
        OnPlayEnded(source);
        onFinishCallback?.Invoke(identifier);
    }

    private void OnPlayEnded(AudioSource source)
    {

        playingSources.Remove(source);
        AddSingleSource(ref source, ref availableSources);
        source.name = AvailableName;
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
        runner.name = AvailableName;
        availableSources.Add(runner.gameObject.AddComponent<AudioSource>());
    }

    private void AddSingleSource(ref AudioSource source, ref List<AudioSource> sources)
    {
        if(!sources.Contains(source))
        {
            sources.Add(source);
        }
    }
}
}