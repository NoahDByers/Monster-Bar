using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum HitResult
{
    Perfect,
    Good,
    Miss
}

public class LaneController : MonoBehaviour
{
    [Header("Lane")]
    [SerializeField] private int laneIndex;
    [SerializeField] private Key key;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private HitFlash hitFlash;
    [SerializeField] private Color laneColor = Color.white;

    [Header("Judgement Windows (seconds)")]
    [SerializeField] private float perfectWindowSec = 0.045f;
    [SerializeField] private float goodWindowSec = 0.09f;
    [SerializeField] private float missWindowSec = 0.140f;

    [Header("References")]
    [SerializeField] private NoteSpawner NoteSpawner;
    private SongClock songClock;

    private readonly List<NoteController> notes = new List<NoteController>();
    private readonly HashSet<NoteController> tracked = new HashSet<NoteController>();

    public int LaneIndex => laneIndex;
    public Transform HitPoint => hitPoint;

    public event Action<HitResult, float> OnJudged;

    private void Awake()
    {
        if (NoteSpawner == null)
            NoteSpawner = FindFirstObjectByType<NoteSpawner>();

        if (NoteSpawner != null)
            songClock = NoteSpawner.Clock;

        if (hitFlash == null)
            hitFlash = GetComponentInChildren<HitFlash>(true);
    }

    public void RegisterNote(NoteController note)
    {
        if (note == null) return;
        if (tracked.Contains(note)) return;
        tracked.Add(note);
        notes.Add(note);
        notes.Sort((a, b) => a.Note.startMs.CompareTo(b.Note.startMs));
    }

    public void UnregisterNote(NoteController note)
    {
        if (note == null) return;
        tracked.Remove(note);
        notes.Remove(note);
    }

    private void Update()
    {
        if (songClock == null || !songClock.Started) return;

        float songTimeMs = songClock.NowMs;

        SyncNotes();
        CullMisses(songTimeMs);

        if (WasKeyPressed())
        {
            hitFlash?.BeginHold();
            JudgeClosest(songTimeMs);
        }

        if (WasKeyReleased())
            hitFlash?.EndHold();
    }

    private bool WasKeyPressed()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null || key == Key.None) return false;
        return keyboard[key].wasPressedThisFrame;
    }

    private bool WasKeyReleased()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null || key == Key.None) return false;
        return keyboard[key].wasReleasedThisFrame;
    }

    private void SyncNotes()
    {
        // Remove notes that have been returned to the pool or destroyed.
        for (int i = notes.Count - 1; i >= 0; i--)
        {
            var note = notes[i];
            if (note == null || !note.gameObject.activeInHierarchy)
            {
                notes.RemoveAt(i);
                if (note != null) tracked.Remove(note);
            }
        }

        // Fallback: discover active notes in this lane even if spawner registration was missed.
        var activeNotes = FindObjectsOfType<NoteController>(false);
        foreach (var note in activeNotes)
        {
            if (note.Note.lane != laneIndex) continue;
            if (tracked.Contains(note)) continue;
            RegisterNote(note);
        }
    }

    private void CullMisses(float songTimeMs)
    {
        float missWindowMs = missWindowSec * 1000f;

        while (notes.Count > 0)
        {
            var note = notes[0];
            float deltaMs = songTimeMs - note.Note.startMs;
            if (deltaMs <= missWindowMs) break;

            UnregisterNote(note);
            OnJudged?.Invoke(HitResult.Miss, deltaMs);
            note.RecycleToPool();
        }
    }

    private void JudgeClosest(float songTimeMs)
    {
        if (notes.Count == 0) return;

        var note = notes[0];
        float deltaMs = Mathf.Abs(songTimeMs - note.Note.startMs);

        float perfectWindowMs = perfectWindowSec * 1000f;
        float goodWindowMs = goodWindowSec * 1000f;
        float missWindowMs = missWindowSec * 1000f;

        HitResult result;
        if (deltaMs <= perfectWindowMs) 
            result = HitResult.Perfect;
        else if (deltaMs <= goodWindowMs) 
            result = HitResult.Good;
        else if (deltaMs <= missWindowMs) 
            result = HitResult.Miss;
        else
            return;

        Debug.Log(result);
        UnregisterNote(note);
        OnJudged?.Invoke(result, deltaMs);
        note.RecycleToPool();
    }
}
