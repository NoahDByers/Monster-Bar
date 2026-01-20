using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Glyph Library", fileName = "GlyphLibrary")]
public class GlyphLibrary : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        [Tooltip("Control path key like: d, f, j, k, leftTrigger, rightShoulder, etc.")]
        public string controlPath;

        public Sprite sprite;
    }

    [Header("Glyph Entries")]
    [SerializeField] private List<Entry> entries = new();

    // Built at runtime for fast lookup
    private Dictionary<string, Sprite> _map;

    /// <summary> Returns a glyph sprite for the given controlPath (case-insensitive).</summary>

    public Sprite Get(string controlPath)
    {
        if (string.IsNullOrWhiteSpace(controlPath))
            return null;

        EnsureBuilt();
        _map.TryGetValue(Normalize(controlPath), out var sprite);
        return sprite;
    }

    /// <summary> Ensures the internal map is built. </summary>
    private void EnsureBuilt()
    {
        if (_map != null) return;

        _map = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.controlPath) || entry.sprite == null) continue;

            var key = Normalize(entry.controlPath);

            //last one wins if there is a duplicate
            _map[key] = entry.sprite;
        }
    }

    private static string Normalize(string s) => s.Trim();

    #if UNITY_EDITOR
    private void OnValidate()
    {
        //Rebuilds if it is changed in the inspector
        _map = null;
    }
    #endif
}
