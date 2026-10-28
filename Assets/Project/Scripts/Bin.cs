using UnityEngine;
using UnityEngine.Events;

public class Bin : MonoBehaviour
{
    [SerializeField] private string correctTag;
    
    [SerializeField] private UnityEvent onCorrectTrash;
    [SerializeField] private UnityEvent onIncorrectTrash;

    [Header("Particle Effect")]
    [SerializeField] private ParticleSystem binParticleEffect;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioSource binAudioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Plastic") && !other.CompareTag("Cardboard") && !other.CompareTag("Metal") && !other.CompareTag("Other")) return;

        if (other.CompareTag(correctTag))
        {
            onCorrectTrash.Invoke();
            Destroy(other.gameObject);
            PlayBinEffect(true);
            PlayBinSound(true);
        }
        else
        {
            onIncorrectTrash.Invoke();
            Destroy(other.gameObject);
            PlayBinEffect(false);
            PlayBinSound(false);
        }
    }

    private void PlayBinEffect(bool correct)
    {
        if (binParticleEffect == null) return;

        var main = binParticleEffect.main;
        main.startColor = correct ? correctColor : incorrectColor;

        binParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        binParticleEffect.Play();
    }

    private void PlayBinSound(bool correct)
    {
        if (binAudioSource == null) return;

        AudioClip sound = correct ? correctSound : incorrectSound;

        if (sound != null) binAudioSource.PlayOneShot(sound);
    }
}
