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

    private Vector3 bodyBaseScale;
    private Camera cam;
    private float worldUnitsPerPixel;

    public ManiaNote Note => note;

    public void SetPool(NotePool pool) => this.pool = pool;
    public void RecycleToPool()
    {
        if (pool != null) pool.ReturnToPool(this);
        else gameObject.SetActive(false);
    }

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

        if (body != null) 
            bodyBaseScale = body.localScale;

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

        if (body != null)
        {
            body.localScale = bodyBaseScale;
            body.localPosition = Vector3.zero;
        }

        if (bodyRenderer != null)
            bodyRenderer.enabled = note.IsHold;

        float tMs = (clock != null) ? clock.NowMs : 0f;
        ApplyPose(tMs);
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

    private void ApplyPose(float songTimeMs)
    {
        float x = SnapX(laneX);

        // Head position uses start time
        float headUntil = note.startMs - songTimeMs;
        float headY = hitY + headUntil * unitsPerMs;

        // Put root at head position, children in local space
        transform.position = new Vector3(x, headY, 0f);
        if (head != null) head.localPosition = Vector3.zero;

        if (!note.IsHold || body == null) return;

        float tailUntil = note.endMs - songTimeMs;
        float tailY = hitY + tailUntil * unitsPerMs;

        // Tail relative to head
        float deltaY = tailY - headY;

        // Body centered between head and tail, scaled to length
        body.localPosition = new Vector3(0f, deltaY * 0.5f, 0f);

        var s = bodyBaseScale;
        s.y = Mathf.Max(0.001f, Mathf.Abs(deltaY));
        body.localScale = s;
    }
}
