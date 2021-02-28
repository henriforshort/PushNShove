using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour {
    [Header("Self References")]
    public AudioSource musicAudioSource;
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

        try {
            GetAudioSource(pitch).PlayOneShot(GetAudioClip(audioPath, filename, index), volume);
        } catch {
            Debug.LogWarning("sound not found: "+audioPath+" "+filename);
        }
    }

    public AudioClip GetAudioClip(string audioPath, string filename, int index = -1) {
        filename = filename.Replace("_", " ").ToCamelCase();
        if (index > 0) filename += "*" + index;
        return Resources.Load<AudioClip>(Directory
            .GetFiles(resourcesPath + audioPath, filename + "*", SearchOption.AllDirectories)
            .Where(p => !p.Contains(".meta"))
            .Random()
            ?.Remove(resourcesPath)
            ?.Remove(".wav"));
    }

    public AudioSource GetAudioSource(Pitch pitch) {
        return pitch switch {
            Pitch.LOW => lowPitchedAudioSource,
            Pitch.HIGH => highPitchedAudioSource,
            _ => normalPitchedAudioSource
        };
    }
}