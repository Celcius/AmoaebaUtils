using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using AmoaebaUtils;
using System;

namespace AmoaebaUtils
{
public class AudioMixerSoundSystem : SoundSystem
{
    [SerializeField]
    AudioMixer mainMixer;
    public AudioMixer MainMixer => mainMixer;

    [SerializeField]
    private string mainAttenuation = "MainAttenuation";

    Dictionary<string, AudioMixerGroup> groupsByName = new Dictionary<string, AudioMixerGroup>();
    Dictionary<string, AudioMixerSnapshot> snapshotsByName = new Dictionary<string, AudioMixerSnapshot>();

    public override float MainVolume
    {
        set  
        { 
            base.MainVolume = 1.0f;
            mainMixer.SetFloat(mainAttenuation, value);
        }

        get { return base.MainVolume; }
    }


    protected override void OnEnable() 
    {
        base.OnEnable();

        groupsByName.Clear();
        snapshotsByName.Clear();

        if(MainMixer == null)
        {
            return;
        }

        this.MainVolume = MainVolume; // Refresh volume;

        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups(string.Empty);
        foreach(AudioMixerGroup group in groups)
        {
            groupsByName[group.name] = group;
        }
        
        AudioMixerSnapshot[] snapshots = (AudioMixerSnapshot[])MainMixer.GetType().GetProperty("snapshots").GetValue(MainMixer, null);
        foreach(AudioMixerSnapshot snapshot in snapshots)
        {
            snapshotsByName[snapshot.name] = snapshot;
        }
    }

    public AudioMixerGroup GetGroup(string name)
    {
        if(groupsByName.ContainsKey(name))
        {
            return groupsByName[name];
        }

        return null;
    }

    public AudioMixerSnapshot GetSnapshot(string name)
    {
        if(snapshotsByName.ContainsKey(name))
        {
            return snapshotsByName[name];
        }

        return null;
    }

    public bool TransitionToSnapshot(string name, float transitionTime)
    {
        if(snapshotsByName.ContainsKey(name))
        {
            snapshotsByName[name].TransitionTo(transitionTime);
            return true;
        }
        return false;
    }
}
}