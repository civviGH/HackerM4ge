using UnityEngine;
using System.Collections;

public static class AudioFadeOut {

    public static IEnumerator FadeOut (AudioSource audioSource, float fadeTime, GameObject objectToDestroy = null) {
        float startVolume = audioSource.volume;
        Debug.Log (startVolume);
        while (audioSource.volume > 0f) {
            audioSource.volume -= startVolume * 1f/60f / fadeTime;
            Debug.Log (audioSource.volume);
            yield return new WaitForSeconds(1f/60f);
        }
        Debug.Log (audioSource.volume);
        audioSource.Stop ();
        if (objectToDestroy != null) {
            UnityEngine.Object.Destroy (objectToDestroy);
        }
    }
}