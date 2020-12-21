using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OldSine : MonoBehaviour {
    [Header("Balancing")]
    public Style style;
    public float period;
    public float amplitude;
    [Header("Balancing but boring")]
    public float valueBoost;
    [Range(-1, 1)] public float startOffset;

    [Header("State")]
    public float startDate;
    public float startValue;
    public float currentValue;
    
    [Header("Self References")]
    public Graphic graphicComponent;

    public enum Style { VERTICAL, HORIZONTAL, ALPHA }

    public void Start() {
        if (period == 0) period = 1;
        startDate = Time.time;
        
        if (style == Style.HORIZONTAL) startValue = transform.position.x;
        if (style == Style.VERTICAL) startValue = transform.position.y;
        if (style == Style.ALPHA) startValue = graphicComponent.color.a;
        startValue += valueBoost;
    }

    private void Update() {
        currentValue = startValue + 
                       Mathf.Sin(((Time.time - startDate)/period + startOffset/4)*(2 * Mathf.PI)) * amplitude;
        
        if (style == Style.HORIZONTAL) this.SetX(currentValue);
        if (style == Style.VERTICAL) this.SetY(currentValue);
        if (style == Style.ALPHA) graphicComponent.SetAlpha(currentValue);
    }
}
