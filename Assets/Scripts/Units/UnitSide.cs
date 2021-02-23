using UnityEngine;

public class UnitSide : MonoBehaviour {
    private Unit _unit;
    public Unit unit => _unit ?? (_unit = GetComponent<Unit>());
    
    
    protected virtual void Init() { }
    protected virtual void Die() { }
    protected virtual void GetDefeated() { }

    public void Awake() {
        Init();
        
        unit.onDeath.AddListener(Die);
        unit.onDefeat.AddListener(GetDefeated);
    }
}