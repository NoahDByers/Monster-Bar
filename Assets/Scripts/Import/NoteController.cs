using System;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    private ManiaNote note;
    private SongClock clock;
    private float laneX;
    private float hitY;
    private float unitsPerMs;

    private NotePool pool;
    private Camera cam;
    private float worldUnitsPerPixel;

    public void SetPool(NotePool pool) => this.pool = pool;

    // Initialization methods
    private void Awake()
    {
        cam = Camera.main;
        if (cam != null && cam.orthographic)
            worldUnitsPerPixel = (cam.orthographicSize * 2f) / Screen.height;
    }
    public void Init(ManiaNote note, SongClock clock, float laneX, float hitY, float unitsPerMs)
    {
        this.note = note;
        this.clock = clock;
        this.laneX = laneX;
        this.hitY = hitY;
        this.unitsPerMs = unitsPerMs;
    }

    //Helper to snap the notes to a pixel based grid
    private Vector3 SnapToPixelGrid(Vector3 pos)
    {
        if (worldUnitsPerPixel <= 0f) return pos;
        pos.x = Mathf.Round(pos.x / worldUnitsPerPixel) * worldUnitsPerPixel;
        pos.y = Mathf.Round(pos.y / worldUnitsPerPixel) * worldUnitsPerPixel;
        return pos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (clock == null) return;

        float songTimeMs = clock.NowMs;
        float timeUntilHitMs = note.startMs - songTimeMs;

        // Position based on time (stays synced)
        float y = hitY + timeUntilHitMs * unitsPerMs;
        var pos = new Vector3(laneX, y, 0f);
        transform.position = SnapToPixelGrid(pos);

        // Destroy after it passes the hit line by a bit
        if (timeUntilHitMs < -250f)
        {
            if (pool != null) 
                pool.ReturnToPool(this);
            else
                gameObject.SetActive(false);
        }
    }
}
