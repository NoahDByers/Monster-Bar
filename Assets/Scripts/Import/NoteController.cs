using UnityEngine;

public class NoteController : MonoBehaviour
{
    private ManiaNote note;
    private SongClock clock;
    private float laneX;
    private float hitY;
    private float unitsPerMs;

    private NotePool pool;

    public void SetPool(NotePool pool) => this.pool = pool;

    // Initialization method
    public void Init(ManiaNote note, SongClock clock, float laneX, float hitY, float unitsPerMs)
    {
        this.note = note;
        this.clock = clock;
        this.laneX = laneX;
        this.hitY = hitY;
        this.unitsPerMs = unitsPerMs;
    }

    // Update is called once per frame
    void Update()
    {
        if (clock == null) return;

        float songeTimeMs = clock.NowMs;
        float timeUntilHitMs = note.startMs - songeTimeMs;

        // Position based on time (stays synced)
        float y = hitY + timeUntilHitMs * unitsPerMs;
        transform.position = new Vector3(laneX, y, 0f);

        // Destroy after it passes the hit line by a bit
        if (timeUntilHitMs < 0f)
        {
            if (pool != null) 
                pool.ReturnToPool(this);
            else
                gameObject.SetActive(false);
        }
    }
}
