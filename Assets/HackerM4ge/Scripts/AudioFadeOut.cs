using UnityEngine;
using System.Collections;

public static class AudioFadeOut {

    public static IEnumerator FadeOut (AudioSource audioSource, float fadeTime, GameObject objectToDestroy = null) {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0f) {
            audioSource.volume -= startVolume * 1f/60f / fadeTime;
            yield return new WaitForSeconds(1f/60f);
        }
        audioSource.Stop ();
        if (objectToDestroy != null) {
            UnityEngine.Object.Destroy (objectToDestroy);
        }
    }
}