using UnityEngine;

public class DoubleJumpRing : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.4f;
    [SerializeField] private float startScale = 0.3f;
    [SerializeField] private float endScale = 1.0f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve alphaCurve;

    private SpriteRenderer spriteRenderer;
    private float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;

        float scale = Mathf.Lerp(startScale, endScale, scaleCurve.Evaluate(t));
        transform.localScale = Vector3.one * scale;

        Color color = spriteRenderer.color;
        color.a = alphaCurve.Evaluate(t);
        spriteRenderer.color = color;

        if (timer >= 1f)
        {
            Destroy(gameObject);
        }
    } 
}
