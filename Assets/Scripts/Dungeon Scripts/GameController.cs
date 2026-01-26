using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField]private AudioManager audioManager;

    void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("GameController: Music AudioSource is not assigned.");
        }

        musicSource.Play();
    }

    
}
