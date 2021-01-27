using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioSource audioSource;
    public List<SoundCategory> sounds;

    public void Play(SoundType soundType, int index = -1) {
        SoundCategory category = sounds.FirstOrDefault(c => c.type == soundType);
        if (category == null) {
            Debug.LogError("Cannot play sound, category : "+soundType+" doesn't exist");
            return;
        }
        if (category.audioClips == null || category.audioClips.Count == 0) {
            Debug.LogError("Cannot play sound, category : "+soundType+" has no audio clips");
            return;
        }

        AudioClip clipToPlay;
        if (index < 0) clipToPlay = category.audioClips.Random();
        else if (category.audioClips.Count >= index) clipToPlay = category.audioClips[index-1];
        else {
            Debug.LogError("Cannot play sound, category : "+soundType+" doesn't have "+
                           index+" audioclips");
            return;
        }
        
        audioSource.PlayOneShot(clipToPlay);
    }

    [Serializable]
    public class SoundCategory {
        public SoundType type;
        public List<AudioClip> audioClips;
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
    WEAPON_DRAW_METAL
}