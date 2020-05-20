using UnityEngine;

namespace AmoaebaUtils
{
public class CoroutineRunner : MonoBehaviour
{
    public delegate void OnApplicationPauseHandler(bool focusStatus);
    public event OnApplicationPauseHandler OnUnityApplicationPause;

    public delegate void OnApplicationQuitHandler();
    public event OnApplicationQuitHandler OnUnityApplicationQuit;
    
    public static CoroutineRunner Instantiate(string ownerName)
    {
        GameObject gameObject = new GameObject(ownerName + "Runner");

        Object.DontDestroyOnLoad(gameObject);

        return gameObject.AddComponent<CoroutineRunner>();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        this.OnUnityApplicationPause?.Invoke(pauseStatus);
    }

    private void OnApplicationQuit() {
        this.OnUnityApplicationQuit?.Invoke();
    }
}
}
