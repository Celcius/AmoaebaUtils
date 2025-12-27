using UnityEngine;
using UnityEditor;
using AmoaebaUtils;
using NUnit.Framework;

public class LoadTwineToDialogrScene : ScriptableWizard
{
    [SerializeField]
    private TextAsset[] assetsToLoad;

    [SerializeField]


    [MenuItem("Assets/Create/Dialogr/Load Twine to Dialogr Scene",false,(int)'a')]
    public static void ShowWizard()
    {
        
        ScriptableWizard.DisplayWizard<LoadTwineToDialogrScene>("Load Twine TxT Files", "Load");
    }

    private void OnWizardCreate()
    {
        if(assetsToLoad == null || assetsToLoad.Length == 0)
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

        foreach(TextAsset asset in assetsToLoad)
        {
            DialogrSceneScriptable scriptable = ScriptableObject.CreateInstance<DialogrSceneScriptable>();
            scriptable.InitializeAsset(asset);
            ScriptableObjectUtility.CreateAssetAtPath(scriptable, directory+"/"+asset.name+ ".asset");    
        }
    }
}
