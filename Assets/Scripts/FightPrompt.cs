
using UnityEngine;
using UnityEngine.UI;

public class FightPrompt : MonoBehaviour {
    public float startTime;
    public Text battleText;
    
    public void Start() {
        battleText.text = "Battle " + G.m.s.battle;
        startTime = Time.time;
    }

    public void Update() {
        if (Time.time - startTime > 1) {
            B.m.gameState = B.State.PLAYING;
            Destroy(gameObject);
        }
    }
}
