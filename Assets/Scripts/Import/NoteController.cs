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

    [Header("Visuals")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private SpriteRenderer bodyRenderer;

    private Camera cam;
    private float worldUnitsPerPixel;

    public void SetPool(NotePool pool) => this.pool = pool;

    // Initialization methods
    private void Awake()
    {
        //This can be set in the inspector, but this should auto find it
        if (head == null)
        {
            var t = transform.Find("Head");
            if (t != null) head = t;
        }

        if (body == null)
        {
            var t = transform.Find("Body");
            if (t != null) body = t;
        }

        if (bodyRenderer == null && body != null)
            bodyRenderer = body.GetComponent<SpriteRenderer>();

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
    private float SnapX(float x)
    {
        if (worldUnitsPerPixel <= 0f) return x;
        x = Mathf.Round(x / worldUnitsPerPixel) * worldUnitsPerPixel;
        return x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (clock == null) return;

        float songTimeMs = clock.NowMs;

        float headTimeUntil = note.startMs - songTimeMs;
        float headY = hitY + headTimeUntil * unitsPerMs;

        // Position based on time (stays synced)
        float x = SnapX(laneX);
        
        if (head != null)
            head.position = new Vector3(x, headY, 0f);
        else
            transform.position = new Vector3(x, headY, 0f); //Only calls if there isn't a head for the note

        if (note.IsHold)
        {
            // Tail position uses endMs
            float tailTimeUntil = note.endMs - songTimeMs;
            float tailY = hitY + tailTimeUntil * unitsPerMs;

            // Setting the body dimensions based on the had and tail
            float minY = Mathf.Min(headY, tailY);
            float maxY = Mathf.Max(headY, tailY);
            float length = maxY - minY;

            if (body != null)
            {
                // Calculates the midway between the head and tail and puts the body there
                body.position = new Vector3(x, (minY + maxY) * 0.5f, 0f);

                // Scales the body to match the required length
                var s = body.localScale;
                s.y = Mathf.Max(0.001f, length);
                body.localScale = s;
            }
        }

        // Despawn logic:
        // - Tap: when start passes by grace
        // - Hold: when end passes by grace

        const float despawnGraceMs = 250f;

        float refTimeUntil = note.IsHold ? (note.endMs - songTimeMs) : (note.startMs - songTimeMs);
        if (refTimeUntil < -despawnGraceMs)
        {
            if (pool != null) pool.ReturnToPool(this);
            else gameObject.SetActive(false);
        }
    }
}
