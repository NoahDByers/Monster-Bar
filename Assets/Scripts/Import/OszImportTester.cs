using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;

public class OszImportTester : MonoBehaviour
{

    [SerializeField] private string oszPath;
    [SerializeField] private NoteSpawner spawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SongClock clock;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (string.IsNullOrWhiteSpace(oszPath) || !File.Exists(oszPath))
        {
            Debug.LogError("OSZ path invalid");
            return;
        }

        if (spawner == null || audioSource == null)
        {
            Debug.LogError("Assign spawner and audioSource in Inspector.");
            return;
        }

        // Read chartts + keep the winning .osu text so we can get AudioFilename
        ManiaChart bestChart = null;
        string bestOsuText = null;

        using (ZipArchive archive = ZipFile.OpenRead(oszPath))
        {
           foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith(".osu", StringComparison.OrdinalIgnoreCase))
                    continue;

                using (var reader = new StreamReader(entry.Open()))
                {
                    string text = reader.ReadToEnd();

                    if (!text.Contains("Mode: 3")) continue;

                    ManiaChart chart;
                    try { chart = OsuManiaParser.Parse(text); }
                    catch { continue; }

                    if (bestChart == null || chart.notes.Count > bestChart.notes.Count)
                    {
                        bestChart = chart;
                        bestOsuText = text;
                    }  
                }
            } 

            if (bestChart == null)
            {
               Debug.LogError("No Osu!mania charts found in this OSZ file.");
               return; 
            }

            Debug.Log($"Selected chart: keys={bestChart.keyCount}, notes={bestChart.notes.Count}");

            // Get audio file name from the selected .osu
            string audioFilename = OsuManiaParser.GetAudioFilename(bestOsuText);
            if (string.IsNullOrEmpty(audioFilename))
            {
                Debug.LogError("AudioFilename not found in .osu [General].");
                return;
            }

            //Find that audio file inside the OSZ
            ZipArchiveEntry audioEntry = null;
            foreach (var e in archive.Entries)
            {
                // .osu often uses just the filename; compare against FullName and file name
                if (string.Equals(e.FullName, audioFilename, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(Path.GetFileName(e.FullName), audioFilename, StringComparison.OrdinalIgnoreCase))
                {
                    audioEntry = e;
                    break;
                }
            }

            if (audioEntry == null)
            {
                Debug.LogError($"Audio file not found in OSZ: {audioFilename}");
                return;
            }

            // Extract the the audio file to a persistent data path
            string outDir = Path.Combine(Application.persistentDataPath, "ImportedOszAudio");
            Directory.CreateDirectory(outDir);

            string outPath = Path.Combine(outDir, Path.GetFileName(audioEntry.FullName));
            using (var inStream = audioEntry.Open())
            using (var outStream = File.Create(outPath))
            {
                inStream.CopyTo(outStream);
            }

            // Load chart into spawner now (it will start spawning after audio plays)
            spawner.LoadChart(bestChart);

            //Load and play audio clip
            StartCoroutine(LoadAndPlayAudio(outPath));

        }
    }

    private System.Collections.IEnumerator LoadAndPlayAudio(string filePath)
    {
        string url = "file://" + filePath.Replace("\\", "/");

        // Choose AudioType based on extension (more reliable than UNKNOWN)
        AudioType type = AudioType.UNKNOWN;
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        if (ext == ".mp3") type = AudioType.MPEG;
        else if (ext == ".ogg") type = AudioType.OGGVORBIS;
        else if (ext == ".wav") type = AudioType.WAV;

        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, type))
        {
            // Important: avoid streaming decode stutters
            var dh = (DownloadHandlerAudioClip)req.downloadHandler;
            dh.streamAudio = false;
            dh.compressed = false;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio load failed: " + req.error);
                yield break;
            }

            var clip = DownloadHandlerAudioClip.GetContent(req);

            // Force Unity to load/decode audio data fully before starting playback
            clip.LoadAudioData();
            while (clip.loadState == AudioDataLoadState.Loading)
                yield return null;

            audioSource.clip = clip;

            clock.SetAudioSource(audioSource);
            clock.StartSongNow();
        }
    }

}
