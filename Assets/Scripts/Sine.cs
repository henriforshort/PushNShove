using TMPro;
using UnityEngine;

public class Sine : MonoBehaviour {
    [Header("Balancing")]
    public float period;
    public float amplitude;
    public float valueOffset;
    [Range(-1, 1)] public float timeOffset;
    public Mode mode;
    public TMP_Text graphicComponent;

    [Header("State")]
    public float startDate;
    public float startValue;
    public float currentValue;
    
    public enum Mode { VERTICAL, ALPHA }

    public void Start() {
        startDate = Time.time;
        if (mode == Mode.VERTICAL) startValue = transform.position.y;
        if (mode == Mode.ALPHA) startValue = graphicComponent.color.a;
        startValue += valueOffset;
    }

    private void Update() {
        currentValue = startValue + 
                       Mathf.Sin(((Time.time - startDate)/period + timeOffset/4)*(2 * Mathf.PI)) * amplitude;
        if (mode == Mode.VERTICAL) this.SetY(currentValue);
        if (mode == Mode.ALPHA) graphicComponent.SetAlpha(currentValue);
    }
}
