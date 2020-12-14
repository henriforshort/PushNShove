using UnityEngine;
using UnityEngine.SceneManagement;

public class G : MonoBehaviour {
    [Header("Balancing")]
    public float collideDistance;
    
    [Header("Scene References")]
    public GameObject background;

    [Header("Colors")]
    public Color white = new Color(201, 204, 161);
    public Color yellow = new Color(202, 160, 90);
    public Color orange = new Color(174, 106, 71);
    public Color red = new Color(139, 64, 73);
    public Color black = new Color(84, 51, 68);
    public Color darkGrey = new Color(81, 82, 98);
    public Color grey = new Color(99, 120, 125);
    public Color lightGrey = new Color(142, 160, 145);
    
    public static G m;
    
    public enum SceneName { Battle, StartMenu }

    public void Start() {
        if (m == null) m = this;
        if (m != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }

    public static void StartGame() {
        m.background.SetActive(true);
        m.Wait(0.2f, () => SceneManager.LoadScene(SceneName.Battle.ToString()));
    }
}
