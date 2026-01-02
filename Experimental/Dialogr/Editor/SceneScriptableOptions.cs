using UnityEngine;
using UnityEditor;
using Dialogr;

public class SceneScriptableOptions
{
    [MenuItem("Assets/Reload Dialogr .Twee Assets",false, 99)]
    private static void Reload()
    {
        foreach(Object obj in Selection.objects)
        {
            if(obj.GetType() != typeof(DialogrSceneScriptable))
            {
                continue;
            }
            DialogrSceneScriptable scriptable = (DialogrSceneScriptable)obj;
            scriptable.ReloadTweeAsset();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = Selection.objects[Selection.count-1];
        EditorUtility.FocusProjectWindow();

    }

    // The validation function (note the 'true' parameter in the MenuItem attribute)
    // This function runs automatically to determine the state of the menu item
    [MenuItem("Assets/Reload Dialogr .Twee Assets", true, 99)] 
    private static bool ReloadValidation()
    {
        if(Selection.objects == null)
        {
            return false;    
        }
        // Check if a single object is selected
        if (Selection.objects.Length < 0)
        {
            return false;
        }

        foreach(Object obj in Selection.objects)
        {
            if(obj.GetType() != typeof(DialogrSceneScriptable))
            {
                return false;
            }
        }
        return true;
    }    
}
