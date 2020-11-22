using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCluster : MonoBehaviour{
    private void Start() {
        foreach (Transform t in transform) {
            t.SetX(Random.Range(G.m.enemySpawnPosXRange.x, G.m.enemySpawnPosXRange.y));
        }
    }
}