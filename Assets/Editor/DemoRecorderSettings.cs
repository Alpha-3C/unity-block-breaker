using UnityEngine;

[CreateAssetMenu(menuName = "Study/Demo Recorder Settings")]
public sealed class DemoRecorderSettings : ScriptableObject
{
    [SerializeField, Tooltip("Record a demo movie whenever Play mode starts.")]
    private bool autoRecordOnPlay;

    public bool AutoRecordOnPlay => autoRecordOnPlay;
}
