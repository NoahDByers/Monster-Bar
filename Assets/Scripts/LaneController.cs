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

public  partial class LaneController : MonoBehaviour
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
    [SerializeField] private SongClock songClock;

    [Header("Sounds")]
    [SerializeField] private AudioSource hitSound;

    private SongStatManager songStatManager;
    private readonly List<NoteController> notes = new List<NoteController>();
    private readonly HashSet<NoteController> tracked = new HashSet<NoteController>();

    public int LaneIndex => laneIndex;
    public Transform HitPoint => hitPoint;

    public event Action<HitResult, float> OnJudged;

    private void Awake()
    {
        if (NoteSpawner == null) NoteSpawner = FindFirstObjectByType<NoteSpawner>();       
        if (songStatManager == null) songStatManager = FindFirstObjectByType<SongStatManager>();
        if (hitFlash == null) hitFlash = GetComponentInChildren<HitFlash>(true);
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
            songStatManager.RegisterHit(HitResult.Miss);
            OnJudged?.Invoke(HitResult.Miss, deltaMs);
            note.RecycleToPool();
        }
    }

    private void JudgeClosest(float songTimeMs)
    {
        if (notes.Count == 0) return;

        int idx = FindClosesNoteIndex(songTimeMs);
        if (idx < 0 || idx >= notes.Count) return;

        var note = notes[idx];
        float deltaMs = Mathf.Abs(songTimeMs - note.Note.startMs);

        float perfectWindowMs = perfectWindowSec * 1000f;
        float goodWindowMs = goodWindowSec * 1000f;
        float missWindowMs = missWindowSec * 1000f;

        HitResult result;
        if (deltaMs <= perfectWindowMs) {
            result = HitResult.Perfect;
            hitSound.Play();
        }
        else if (deltaMs <= goodWindowMs) {
            result = HitResult.Good;
            hitSound.Play();
        }
        else if (deltaMs <= missWindowMs)
        {
            result = HitResult.Miss;
        } 
        else
            return;

        songStatManager.RegisterHit(result);
        UnregisterNote(note);
        OnJudged?.Invoke(result, deltaMs);
        note.RecycleToPool();
    }

    private int FindClosesNoteIndex(float songTimeMs)
    {
        int count = notes.Count;
        if (count == 0) return -1;

        //Looking for where startMs >= songTimeMs
        int lo = 0;
        int hi = count - 1;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) / 2);
            float t = notes[mid].Note.startMs;

            if (t < songTimeMs)
                lo = mid + 1;
            else
                hi = mid - 1;
        }

        //lo is the first "after" index, lo - 1 is "before"
        int after = lo;
        int before = lo - 1;

        if (before < 0) return after;
        if (after >= count) return before;

        float dBefore = Mathf.Abs(songTimeMs - notes[before].Note.startMs);
        float dAfter = Math.Abs(songTimeMs - notes[after].Note.startMs);

        //Tie break towards the earlier note
        return (dBefore <= dAfter) ? before : after;
    }
}

public partial class LaneController
{
    public void OnLaneAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            hitFlash.BeginHold();
            if (songClock != null && songClock.Started)
                JudgeClosest(songClock.NowMs);
        }

        if (ctx.canceled)
        {
            hitFlash.EndHold();
        }
    }
}
