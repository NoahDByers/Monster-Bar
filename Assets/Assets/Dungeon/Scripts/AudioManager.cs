using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public void PlaySFX(AudioClip audioClip, float volume = 14)
    {
        StartCoroutine(PlaySFXCoroutine(audioClip, volume));
    }

    IEnumerator PlaySFXCoroutine(AudioClip audioClip, float volume = 14)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioClip.length + 2);

        Destroy(audioSource);
    }
}
