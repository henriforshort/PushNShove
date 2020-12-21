using TMPro;
using UnityEngine;

public class HpLossUi : MonoBehaviour {
    [Header("Balancing")]
    public float targetY;
    public float speed;
    public float lifetime;
    public float fadeDuration;
    
    [Header("State")]
    public float initialY;
    public float expirationDate;
    
    [Header("References")]
    public TMP_Text number;
    public Unit unit;

    public void Start() {
        initialY = transform.position.y;
        expirationDate = Time.time + lifetime;
    }

    //Move up, slower and slower
    //After (lifetime) seconds, get progressively transparent in (fadeDuration) seconds, then get destroyed
    public void Update() {
        transform.LerpYTo(initialY + targetY, speed);
        
        if (Time.time > expirationDate) {
            number.alpha = Time.time.Prel(expirationDate + fadeDuration, expirationDate);
        }

        if (Time.time > (expirationDate + fadeDuration)) {
            unit.hpLossUis.Remove(this);
            Destroy(gameObject);
        }
    }
}
