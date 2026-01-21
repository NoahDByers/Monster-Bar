using UnityEngine;

public struct SongStats
{
    public int perfectCount, goodCount, missCount, maxCombo, totalNotes, hitNotes;
    public float accuracy, score;
}
public class SongStatManager : MonoBehaviour
{
    private int perfectCount = 0, goodCount = 0, missCount = 0, maxCombo = 0, currentCombo = 0, totalNotes = 0, hitNotes = 0;
    
    // weight
    private float wPerfect = 1f, wGood = 0.75f; 
    private float accuracy = 0f, score = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterHit(HitResult accuracyType)
    {
        float scoreWeight = 0f;
        switch (accuracyType)
        {
            case HitResult.Perfect:
                perfectCount++;
                currentCombo++;
                hitNotes++;
                scoreWeight = wPerfect;
                break;
            case HitResult.Good:
                goodCount++;
                currentCombo++;
                hitNotes++;
                scoreWeight = wGood;
                break;
            default:
                missCount++;
                currentCombo = 0;
                break;
        }
        totalNotes++;
        maxCombo = currentCombo > maxCombo ? currentCombo : maxCombo;
        accuracy = (perfectCount * wPerfect + goodCount * wGood) / totalNotes * 100;
        score += currentCombo * (50 * scoreWeight);
    }

    public SongStats getData()
    {
        return new SongStats{
            perfectCount = perfectCount,
            goodCount = goodCount,
            missCount = missCount,
            maxCombo = maxCombo,
            totalNotes = totalNotes,
            hitNotes = hitNotes,
            accuracy = accuracy,
            score = score
        };
    }
}
