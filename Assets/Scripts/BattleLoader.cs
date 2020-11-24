using UnityEngine;

public class BattleLoader : MonoBehaviour { //Plays at the beginning of a battle, before B but after G
    private void Start() {
        G.m.InitRun();
        Destroy(gameObject);
    }
}
