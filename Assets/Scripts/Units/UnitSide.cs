using UnityEngine;

public class UnitSide : MonoBehaviour {
    private Unit _unit;
    public Unit unit => _unit ?? (_unit = GetComponent<Unit>());
    
    
    protected virtual void Init() { }
    protected virtual void OnDeath() { }
    protected virtual void OnDefeat() { }

    public void Awake() {
        Init();
        
        unit.onDeath.AddListener(OnDeath);
        unit.onDefeat.AddListener(OnDefeat);
    }
}