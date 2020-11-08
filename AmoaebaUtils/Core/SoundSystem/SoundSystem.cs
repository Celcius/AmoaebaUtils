using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AmoaebaUtils;


namespace AmoaebaUtils
{
public class SoundSystem : SingletonScriptableObject<SoundSystem>
{
    private struct SoundDefinition
    {
        public AudioClip clip;
        public string identifier;
    }

    private List<AudioSource> availableSources;
    private List<AudioSource> playingSources;
    private List<SoundDefinition> awaitingSlots;

    [SerializeField, Range(0, 1.0f)]
    private float mainVolume;
    public float MainVolume
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

    private int instances = 0;

    private void OnEnable() 
    {
        instances = 0;
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

    public void PlaySound(AudioClip clip, string identifier = "", bool skipOnOverload = true)
    {
        bool hasMaxSources = instances >= maxConcurrentSounds;
        
        if(availableSources.Count == 0 && hasMaxSources)
        {
            if(!skipOnOverload)
            {
                SoundDefinition definition;
                definition.clip = clip;
                definition.identifier = identifier;
                awaitingSlots.Add(definition);
            }
            return;
        }

        if(availableSources.Count == 0 && !hasMaxSources)
        {
            CreateSoundSource();
        }

        Play(clip, identifier);
    }

    private void Play(SoundDefinition definition)
    {
        Play(definition.clip, definition.identifier);
    }

    private void Play(AudioClip clip, string identifier)
    {
        if(availableSources.Count <= 0)
        {
            return;
        }
        AudioSource source = availableSources[0];
        availableSources.Remove(availableSources[0]);
        source.gameObject.name = identifier;
        source.clip = clip;

        CoroutineRunner runner = source.GetComponent<CoroutineRunner>();

        Assert.IsFalse(runner == null, "Trying to play sound from incorrect source");
        
        source.volume = mainVolume;
        source.Play();
        runner.StartCoroutine(PlayRoutine(source, clip));

        
    }

    private IEnumerator PlayRoutine(AudioSource source, AudioClip clip)
    {
        float clipLength = clip.length;
        yield return new WaitForSeconds(clipLength);
        OnPlayEnded(source);
    }

    private void OnPlayEnded(AudioSource source)
    {
        playingSources.Remove(source);
        availableSources.Add(source);

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
        availableSources.Add(runner.gameObject.AddComponent<AudioSource>());
        instances++;
    }
}
}