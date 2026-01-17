using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class OszImportTester : MonoBehaviour
{

    [SerializeField] private string oszPath;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (string.IsNullOrWhiteSpace(oszPath))
        {
            Debug.LogError("Set oszPath in the Inspector.");
            return;
        }

        if (!File.Exists(oszPath))
        {
            Debug.LogError("OSZ not found: " + oszPath);
            return;
        }

        int osuCount = 0;

        using (ZipArchive archive = ZipFile.OpenRead(oszPath))
        {
           foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith(".osu", StringComparison.OrdinalIgnoreCase))
                    continue;

                osuCount++;

                using (var reader = new StreamReader(entry.Open()))
                {
                    string text = reader.ReadToEnd();
                    Debug.Log($"Found .osu: {entry.FullName}\nFirst 200 chars:\n" +
                        text.Substring(0, Mathf.Min(200, text.Length)));
                }
            } 
        }

        Debug.Log($"Done. Found {osuCount} .osu files.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
