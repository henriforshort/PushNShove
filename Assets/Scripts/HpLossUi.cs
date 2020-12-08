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

    private void Start() {
        initialY = transform.position.y;
        expirationDate = Time.time + lifetime;
    }

    public void Update() {
        Vector3 pos = transform.position;
        transform.position = Vector3.Lerp(
            pos, 
            new Vector3(pos.x, initialY + targetY, pos.z), 
            speed / 100);
        
        if (Time.time > expirationDate) {
            number.alpha = Time.time.Prel(expirationDate + fadeDuration, expirationDate);
        }

        if (Time.time > (expirationDate + fadeDuration)) {
            Destroy(gameObject);
        }
    }
}
