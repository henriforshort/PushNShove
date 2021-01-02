using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour { //Game manager, handles the whole game
                            //Should contain global balancing and prefabs (NOT depending on any specific game mode)
                            //Should contain State info that is persisted across the whole game
    [Header("State")]
    public GameSave save;
                            
    [Header("Prefabs")]
    public List<Hero> heroPrefabs;
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
    
    public static Game m;
    
    public enum SceneName { Battle, StartMenu, Camp }

    public void Start() {
        if (m == null) m = this;
        if (m != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        InitGame();
    }
    
    
    // ====================
    // START & END
    // ====================

    public void InitGame() { //Called at the start of each run, before init the first battle
        save.InitGame();
    }
    
    
    // ====================
    // UTILS
    // ====================

    public void LoadScene(SceneName sceneName) {
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
}
