using UnityEngine;
using UnityEditor;
using AmoaebaUtils;
using NUnit.Framework;
using System.Collections.Generic;

public class LoadTwineToDialogrScene : ScriptableWizard
{
    [SerializeField]
    private Object[] tweeAssets;

    [MenuItem("Assets/Create/Dialogr/Load Twine to Dialogr Scene",false,(int)'a')]
    public static void ShowWizard()
    {
        
        ScriptableWizard.DisplayWizard<LoadTwineToDialogrScene>("Load Twine TxT Files", "Load");
    }

    private void OnValidate()
    {
        OnWizardUpdate();        
    }

    private void OnWizardUpdate()
    {
        this.isValid = true;

        if(tweeAssets != null && tweeAssets.Length > 0)
        {
            List<Object> finalList = new List<Object>();
            foreach(Object tweeAsset in tweeAssets)
            {
                string path = AssetDatabase.GetAssetPath(tweeAsset);
                if (!path.EndsWith(".twee"))
                {
                    Debug.LogError("Only .twee files are allowed!");
                }   
                else
                {
                    finalList.Add(tweeAsset);
                }
            }
            tweeAssets = finalList.ToArray();
        }

        this.isValid = tweeAssets != null && tweeAssets.Length > 0;    
    }

    private void OnWizardCreate()
    {
        if(tweeAssets == null || tweeAssets.Length == 0)
        {
            return;
        }

        string directory = EditorUtility.OpenFolderPanel("Select Directory under Assets folder", Application.dataPath, "");
        if(string.IsNullOrEmpty(directory))
        {
            return;
        }
        int start = directory.IndexOf("Assets");
        Assert.True(start >=0, "You need to select a folder under the Assets/ directory");
        directory = directory.Substring(start, directory.Length-start);

        foreach(Object asset in tweeAssets)
        {
            if(asset == null)
            {
                continue;
            }
            DialogrSceneScriptable scriptable = ScriptableObject.CreateInstance<DialogrSceneScriptable>();
            scriptable.InitializeAsset(asset);
            ScriptableObjectUtility.CreateAssetAtPath(scriptable, directory+"/"+asset.name+ ".asset");    
        }
    }
}
