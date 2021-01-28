using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [Header("Self References")]
    public AudioSource audioSource;

    private string resourcesPath = "Assets/Resources/";
    private string audioPath = "Audio/Medieval Combat Sounds";

    public void Play(SoundType soundType, float volume = .5f, int index = -1) {
        string filename = soundType.ToString().ToCamelCaseWithSpaces();
        if (index > 0) filename += "*" + index;
        string path = Directory
            .GetFiles(resourcesPath + audioPath, filename + "*", SearchOption.AllDirectories)
            .Where(p => !p.Contains(".meta"))
            .Random()
            ?.Remove(resourcesPath)
            ?.Remove(".wav");
        // Debug.Log(path);
        audioSource.PlayOneShot(Resources.Load<AudioClip>(path), volume);
        
    }
}

public enum SoundType {
    BAG,
    COIN_AND_PURSE,
    COINS,
    FOLEY_ARMOR,
    LOCKER,
    MAGIC_CHIMES,
    PAGES,
    POTION_AND_ALCHEMY,
    SPECIAL_CLICK,
    UI_TIGHT,
    MAGIC_HEAL,
    MAGIC_WHOOSH,
    BODY_FALL,
    PUNCH_1,
    REALISTIC_PUNCH_1,
    SHIELD_METAL_1,
    SHIELD_WOOD_1,
    BLUNT_SWING_1,
    SWORD_SWING_1,
    WHOOSH_1,
    ARROW_FLY_1,
    ARROW_HIT_METAL_1,
    ARROW_HIT_STONE_1,
    ARROW_HIT_WOOD_1,
    BLUNT_DRAW,
    BLUNT_WEAPON_1,
    BOW_DRAW_1,
    BOW_HANDLE_1,
    METAL_WEAPON_HIT_METAL_1,
    METAL_WEAPON_HIT_STONE_1,
    METAL_WEAPON_HIT_WOOD_1,
    STAB_1,
    WEAPON_DRAW_METAL,
    MAGIC_HOLY,
    WHOOSH_8,
    WHOOSH_6,
    MAGIC_LOOP_FIRE,
    SWORD_SWING_2,
    SWORD_SWING_10
}