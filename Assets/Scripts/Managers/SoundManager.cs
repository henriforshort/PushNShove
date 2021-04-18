using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [Header("Balancing")]
    public List<AudioClip> adventureRPG;
    public List<AudioClip> animals;
    public List<AudioClip> casual;
    public List<AudioClip> human;
    public List<AudioClip> medievalCombat;
    
    [Header("Self References")]
    public AudioSource musicAudioSource;
    public AudioSource lowPitchedAudioSource;
    public AudioSource normalPitchedAudioSource;
    public AudioSource highPitchedAudioSource;

    private List<List<AudioClip>> audioClips;
    public enum Type { ADVENTURE_RPG, ANIMALS, CASUAL, HUMAN, MEDIEVAL_COMBAT, NONE }
    public enum Pitch { HIGH, NORMAL, LOW, RANDOM }

    public void Awake() {
        audioClips = new List<List<AudioClip>> {adventureRPG, animals, casual, human, medievalCombat};
    }

    public void Play(Type type, string filename, float volume = .5f, int index = -1, 
        Pitch pitch = Pitch.NORMAL) {
        if (filename == null) return;
        if (filename == "NONE") return;
        if (type == Type.NONE) return;

        AudioClip audioClip = GetAudioClip(type, filename, index);
        if (audioClip == null) Debug.LogWarning($"audio clip not found: {type}/{filename}");
        else GetAudioSource(pitch).PlayOneShot(audioClip, volume);
    }

    public AudioClip GetAudioClip(Type type, string filename, int index = -1) {
        List<AudioClip> possibleClips = audioClips
            .Get((int) type)
            .Where(c => new Regex(filename.Replace("_", " ").ToCamelCase(), RegexOptions.IgnoreCase).IsMatch(c.name))
            .ToList();
        if (index < 0) return possibleClips.Random();
        else return possibleClips.Get(index-1);
    }

    public AudioSource GetAudioSource(Pitch pitch) {
        if (pitch == Pitch.HIGH) return highPitchedAudioSource;
        if (pitch == Pitch.NORMAL) return normalPitchedAudioSource;
        if (pitch == Pitch.LOW) return lowPitchedAudioSource;
        return this.Random(lowPitchedAudioSource, highPitchedAudioSource, normalPitchedAudioSource);
    }
}