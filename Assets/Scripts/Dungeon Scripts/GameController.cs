using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("GameController: Music AudioSource is not assigned.");
        }

        musicSource.Play();
    }

    
}
