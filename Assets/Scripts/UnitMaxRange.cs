using UnityEngine;

public class UnitMaxRange : MonoBehaviour {
    public Unit unit;

    public void OnTriggerStay(Collider other) {
        unit.LongRangeCollide(other);
    }
}
