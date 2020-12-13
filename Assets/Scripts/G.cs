using UnityEngine;
using UnityEngine.SceneManagement;

public class G : MonoBehaviour {
    public float collideDistance;
    public GameObject background;
    
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
