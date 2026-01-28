using System.Collections;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip footstepSound;
    [SerializeField] private Player player;
    [SerializeField] private float holdTime = 0.35f;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float volume = 0.5f;

    void Start()
    {
        StartCoroutine(PlayFootsteps());
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (player.isRunning && player.isGrounded)
            {
                audioManager.PlaySFX(footstepSound, volume);
            }

            yield return new WaitForSeconds(holdTime);
        }
    }
}
