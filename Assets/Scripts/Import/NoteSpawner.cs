using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] laneSpawnPoints; //Should have one per lane
    [SerializeField] private NotePool notePool;

    [Header("Timing / Scroll")]
    [SerializeField] private SongClock clock;
    [SerializeField] private float spawnLeadTimeMs = 2000f; //Spawn before the note should be hit
    [SerializeField] private float unitsPerMs = 0.002f; // Scroll speed tuning
    [SerializeField] private float hitY = -2f; // Where the hit line is
 
    private Queue<ManiaNote> queue;

    //This function is used to load the chart (aka note queue) from the parsed mania chart
    public void LoadChart(ManiaChart chart)
    {
        queue = new Queue<ManiaNote>(chart.notes);
    }

    // Update is called once per frame
    void Update()
    {
        if (queue == null || clock == null || !clock.Started) return;

        float songTimeMs = clock.NowMs;

        while (queue.Count > 0)
        {
            var next = queue.Peek();
            if (next.startMs - songTimeMs > spawnLeadTimeMs) break;

            queue.Dequeue();
            Spawn(next);
        }
    }

    // Create the note object
    private void Spawn(ManiaNote note)
    {
        if (laneSpawnPoints == null || laneSpawnPoints.Length == 0) return;

        int lane = Mathf.Clamp(note.lane, 0, laneSpawnPoints.Length - 1);
        var laneX = laneSpawnPoints[lane].position.x;

        float songTimeMs = clock.NowMs;
        float timeUntilHitMs = note.startMs - songTimeMs;

        float y = hitY + timeUntilHitMs * unitsPerMs;

        var ctrl = notePool.Get();
        ctrl.transform.position = new Vector3(laneX, y, 0f);
        ctrl.Init(note, clock, laneX, hitY, unitsPerMs);
    }
}
