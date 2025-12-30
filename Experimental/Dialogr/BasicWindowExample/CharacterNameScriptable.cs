using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterNameScriptable", menuName = "Dialogr/CharacterNameScriptable", order = 0)]
public class CharacterNameScriptable : ScriptableObject
{
    [Serializable]
    public struct CharacterNameEntry
    {
        [SerializeField]
        public string key;
        [SerializeField]
        public string labelToShow;
    }

    [SerializeField]
    CharacterNameEntry[] Entries;

    private Dictionary<string,string> EntriesDict = null;

    public string GetLabel(string key)
    {
        if(EntriesDict == null)
        {
            EntriesDict = new Dictionary<string, string>();
            foreach(CharacterNameEntry entry in Entries)
            {
                EntriesDict[entry.key] = entry.labelToShow;
            }
        }
        
        return EntriesDict.ContainsKey(key)? EntriesDict[key] : "Entry not found for " + key;
    }
}
