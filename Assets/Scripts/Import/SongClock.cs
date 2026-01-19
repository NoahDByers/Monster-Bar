using UnityEngine;

public class SongClock : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float visualOffsetMs = 0f;

    private double dspStartTime;
    public float NowMs { get; private set; }
    private bool started;

    public bool Started => started;

    private void Update()
    {
        if (!started) return;
        NowMs = (float)((AudioSettings.dspTime - dspStartTime) * 1000.0) - visualOffsetMs;
    }

    public void SetAudioSource(AudioSource src)
    {
        audioSource = src;
    }

    /// <summary>
    /// Start playback using DSP time as the reference
    /// </summary>
    public void StartSongNow()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogError("SongClock: AudioSource or clip is missing.");
            return;
        }

        //Schedule slightly in the future so dspStartTime and playback align cleanly
        dspStartTime = AudioSettings.dspTime + 0.1;
        audioSource.PlayScheduled(dspStartTime);
        started = true;
    }

    /// <summary>
    /// Current song time in milliseconds since scheduled start
    /// </summary>
    public float SongTimeMs()
    {
        if (!started) return 0f;
        
        return (float)((AudioSettings.dspTime - dspStartTime) * 1000.0);
    }
}
