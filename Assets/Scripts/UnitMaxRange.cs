using UnityEngine;

public class UnitMaxRange : MonoBehaviour {
    public Unit unit;
    public Collider col;

    public void OnTriggerStay(Collider other) {
        unit.LongRangeCollide(other);
    }
}
