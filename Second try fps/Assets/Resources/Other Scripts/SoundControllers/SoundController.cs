using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    //no appropriate tool to trimm clip manually in unty;
    [SerializeField] private float duration;

    private AudioSource audioSource;
    private float clipLength;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        clipLength = audioSource.clip.length;
    }

    public void Play()
    {
        StartCoroutine(SoundCoroutine());
    }

    private IEnumerator SoundCoroutine()
    {
        audioSource.Play();
        yield return new WaitForSeconds(duration);

        audioSource.Stop();
        Invoke(nameof(DeactivateSelf),0);
    }

    private void DeactivateSelf()
    {
        ObjectPooler.ReturnGameObject(this);
    }
}
