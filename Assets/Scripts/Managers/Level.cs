using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Level manager, handles a single level, like battle or startMenu
//Should contain only Balancing and Scene References relative to this level.
//Should contain only State info to be deleted at the end of the level.
public class Level<T> : MonoBehaviour where T : Component {
    public AudioSource audioSource;
    
    public static T m;

    public void Awake() {
        if (m == null) m = GetComponent<T>();
        if (m != GetComponent<T>()) Destroy(gameObject);
    }

    public void Play(AudioClip audioClip) {
        if (audioClip == null) Debug.Log("null");
        else audioSource.PlayOneShot(audioClip);
    }
}
