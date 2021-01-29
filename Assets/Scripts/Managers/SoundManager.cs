using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour {
    [Header("Self References")]
    public AudioSource lowPitchedAudioSource;
    public AudioSource normalPitchedAudioSource;
    public AudioSource highPitchedAudioSource;

    private string resourcesPath = "Assets/Resources/";
    
    public enum Pitch { HIGH, NORMAL, LOW }

    public void Play(string audioPath, string filename, float volume = .5f, int index = -1, 
        Pitch pitch = Pitch.NORMAL) {
        if (filename == null) return;
        if (filename == "NONE") return;
        if (audioPath == null) return;

        filename = filename.Replace("_", " ").ToCamelCase();
        if (index > 0) filename += "*" + index;
        string path = Directory
            .GetFiles(resourcesPath + audioPath, filename + "*", SearchOption.AllDirectories)
            .Where(p => !p.Contains(".meta"))
            .Random()
            ?.Remove(resourcesPath)
            ?.Remove(".wav");

        (pitch switch {
            Pitch.LOW => lowPitchedAudioSource,
            Pitch.HIGH => highPitchedAudioSource,
            _ => normalPitchedAudioSource
        }).PlayOneShot(Resources.Load<AudioClip>(path), volume);
        
    }
}