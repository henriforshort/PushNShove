using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Game : MonoBehaviour { //Game manager, handles the whole game
                            //Should contain global balancing and prefabs
                            //Should contain State info that is persisted across the whole game
    [Header("Balancing")]
    public int secondsToMaxHp;
    [Space(20)]
    public int battlesPerRun;
    public float timeToAutoRestart;
    public Vector2 spawnPosXRangeForMelee;
    public Vector2 spawnPosXRangeForRanged;
    public int heroesPerBattle;

    [Space(20)]
    public float unitMaxSpeed;
    public float attackDistance;
    public float collideDistance;
    public float freezeFrameDuration;
    public float bumpRecoverySpeed;
    public float bumpSpeed;
    public float maxBumpSpeed;
    public float defendSpeed;
    public float absorbSpeed;
    public float boardSize;
    
    [Space(20)]
    public int itemsPerBattle;
    public int maxItemsPerHero;
    public float commonDropChance;
    public float rareDropChance;
    public float leggyDropChance;
    
    [Space(20)]
    public int xpBubblesPerBattle;
    public int firstLevelXp;
    public float levelUpStatBonus;
    public float levelUpXpNeededIncrease;
    public float levelUpXpGainedIncrease;
    public float doubleXpDurationInHours;

    [Space(20)]
    public float gemsPerBattle;

    [Header("State")]
    public GameSave save;
                            
    [Header("Prefabs")]
    public List<UnitHero> heroPrefabs;
    public GameObject xpPrefabSmall;
    public GameObject xpPrefabMedium;
    public GameObject xpPrefabBig;
    public Sprite transparentSprite;
    public Sprite whiteSprite;

    [Header("Colors")]
    public Color white = new Color(201, 204, 161);
    public Color yellow = new Color(202, 160, 90);
    public Color orange = new Color(174, 106, 71);
    public Color red = new Color(139, 64, 73);
    public Color black = new Color(84, 51, 68);
    public Color darkGrey = new Color(81, 82, 98);
    public Color grey = new Color(99, 120, 125);
    public Color lightGrey = new Color(142, 160, 145);

    [Header("Self References")]
    public SoundManager sound;
    
    public static Game m;
    
    public enum SceneName { StartMenu, Battle, Camp }
    
    public string savePath => Application.persistentDataPath + "/save.hiloqo";
    
    
    // ====================
    // BASICS
    // ====================

    public void Awake() {
        if (m == null) m = this;
        if (m != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        
        if (File.Exists(savePath)) LoadFromDevice();
        else save.InitGame();
        
        LoadScene(SceneName.Camp);
    }

    public void OnApplicationQuit() {
        if (save.currentScene == SceneName.Camp) return;
        
        SaveToDevice();
    }
    
    
    // ====================
    // SAVE & LOAD
    // ====================

    public void SaveToDevice() {
        FileStream fileStream = new FileStream(savePath, FileMode.Create);
        new BinaryFormatter().Serialize(fileStream, save);
        fileStream.Close();
    }

    public void LoadFromDevice() {
        FileStream fileStream = new FileStream(savePath, FileMode.Open);
        save = (GameSave)new BinaryFormatter().Deserialize(fileStream);
        fileStream.Close();
    }
    
    
    // ====================
    // UTILS
    // ====================

    public void LoadScene(SceneName sceneName) {
        MuteMusic(false);
        save.currentScene = sceneName;
        SaveToDevice();
        SceneManager.LoadScene(sceneName.ToString());
    }

    public GameObject SpawnFX(GameObject fx, Vector3 position, bool mirrored = false, 
        float duration = -1, Transform holder = null, Vector3 rotation = default) {
        if (holder == null) holder = Battle.m.transform;
        if (rotation == default) rotation = Vector3.zero;
        
        GameObject spawnedFx = Instantiate(fx, position, Quaternion.Euler(rotation), holder);
        if (mirrored) spawnedFx.transform.localScale = new Vector3(-1, 1, 1);
        if (duration > 0) Destroy(spawnedFx, duration);
        return spawnedFx;
    }

    public void PurgeStatModifiers(StatModifier.Scope scope) {
        save.heroes.ForEach(h => 
            h.data.stats.ForEach(s => s.PurgeModifiers(scope)));
    }
    
    
    // ====================
    // SOUND
    // ====================

    public void PlaySound(MedievalCombat medievalCombat, float volume = 0.5f, int index = -1, 
        SoundManager.Pitch pitch = SoundManager.Pitch.NORMAL) => 
        sound.Play("Audio/Medieval Combat Sounds", medievalCombat.ToString(), volume, index, pitch);
    
    public void PlaySound(Animals animals, float volume = 0.5f, int index = -1, 
        SoundManager.Pitch pitch = SoundManager.Pitch.NORMAL) => 
        sound.Play("Audio/Farm Animal Sounds", animals.ToString(), volume, index, pitch);
    
    public void PlaySound(Human human, float volume = 0.5f, int index = -1, 
        SoundManager.Pitch pitch = SoundManager.Pitch.NORMAL) => 
        sound.Play("Audio/Human Sounds", human.ToString(), volume, index, pitch);
    
    public void PlaySound(Casual casual, float volume = 0.5f, int index = -1, 
        SoundManager.Pitch pitch = SoundManager.Pitch.NORMAL) => 
        sound.Play("Audio/Casual Music Pack", casual.ToString(), volume, index, pitch);

    public void PlayMusic(AdventureRPG adventureRpg) {
        try {
            AudioClip clip = sound.GetAudioClip("Audio/2D Adventure RPG Music Pack" , adventureRpg.ToString());
            AudioSource audioSource = sound.musicAudioSource;
            if (audioSource.clip == clip) return;
        
            audioSource.clip = clip;
            audioSource.Play();
        } catch {
            Debug.LogWarning("sound not found: Audio/2D Adventure RPG Music Pack "+adventureRpg);
        }
    }

    public void MuteMusic(bool mute = true) => sound.musicAudioSource.volume = mute ? 0 : .2f;
}
