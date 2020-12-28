using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Game : MonoBehaviour { //Game manager, handles the whole game
                            //Should contain global balancing and prefabs (NOT depending on any specific game mode)
                            //Should contain State info that is persisted across the whole game
    [Header("Prefabs")]
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
    
    public enum SceneName { Battle, StartMenu }

    public void Start() {
        if (m == null) m = this;
        if (m != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        // InitGame();
    }

    public void InitGame() {
        // Run.m.heroPrefabs.ForEach(u => u.unit.InitGame());
    }

    public void LoadScene(SceneName sceneName) {
        SceneManager.LoadScene(sceneName.ToString());
    }

    public GameObject SpawnFX(GameObject fx, Vector3 position, bool mirrored = false, 
        Transform holder = null, float duration = 0.5f, Vector3 rotation = default) {
        if (holder == null) holder = transform;
        if (rotation == default) rotation = Vector3.zero;
        GameObject spawnedFx = Instantiate(fx, position, Quaternion.Euler(rotation), holder);
        if (mirrored) spawnedFx.transform.localScale = new Vector3(-1, 1, 1);
        Destroy(spawnedFx, duration);
        return spawnedFx;
    }
}
