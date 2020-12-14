using UnityEngine;

public class Hero : MonoBehaviour {
    public Unit unit;
    public Sprite image;
    public HeroIcon icon;
    public State state;
    public float ultCooldown;
    
    public enum State { ALIVE, DEAD }

    public void Ult() {
        Debug.Log("ult");
    }
}
