using System.Collections;
using UnityEngine;

public class EnvironmentSound : MonoBehaviour
{
    public AudioClip environmentSound;
    [SerializeField] private Player player;
    [SerializeField] private float holdTime = 0.35f;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float volume = 0.5f;

    void Start()
    {
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        audioManager.PlaySFX(environmentSound, volume);
        yield return new WaitForSeconds(holdTime);
    }
}
