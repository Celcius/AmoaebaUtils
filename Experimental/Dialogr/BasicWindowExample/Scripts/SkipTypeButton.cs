using UnityEngine;

public class SkipTypeButton : MonoBehaviour
{
    [SerializeField]
    private SlowTyper SlowTyper;

    public void OnSkipPress()
    {
        SlowTyper.ForceEnd();
    }
}
