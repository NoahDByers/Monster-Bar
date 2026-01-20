using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    [Header("Timing")]
    [SerializeField] private float duration = 0.08f;

    [Header("Strength")]
    [SerializeField] private float peakOpacity = 1f;
    [SerializeField] private float falloff = 6f;

    static readonly int OpacityId = Shader.PropertyToID("_Opacity");
    static readonly int IntensityId = Shader.PropertyToID("_Falloff");

    MaterialPropertyBlock mpb;
    float t;
    bool playing;
    bool holding;
    float holdStrength = 1f;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        sr.enabled = false;
    }

    public void Play(float strength = 1f)
    {
        t = 0f;
        playing = true;
        holding = false;
        sr.enabled = true;

        ApplyStrength(strength, 1f);
    }

    void Update()
    {
        if (holding)
        {
            ApplyStrength(holdStrength, 1f);
            return;
        }

        if (!playing) return;

        t += Time.unscaledDeltaTime;
        float u = Mathf.Clamp01(t / duration);

        // snappy falloff (ease out)
        float fade = 1f - (u * u);

        ApplyStrength(1f, fade);

        if (u >= 1f)
        {
            playing = false;
            sr.enabled = false;
        }
    }

    public void BeginHold(float strength = 1f)
    {
        holdStrength = strength;
        holding = true;
        playing = false;
        sr.enabled = true;
        ApplyStrength(holdStrength, 1f);
    }

    public void EndHold()
    {
        if (!holding) return;
        holding = false;
        t = 0f;
        playing = true;
    }

    private void ApplyStrength(float strength, float fade)
    {
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(OpacityId, peakOpacity * strength * fade);
        mpb.SetFloat(IntensityId, falloff * strength * fade);
        sr.SetPropertyBlock(mpb);
    }
}
