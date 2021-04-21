using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour {
    [Header("Balancing")]
    public float speed;
    public float maxDistFromUnits;
    public float maxDistFromMapCenter;
    
    [Header("State")]
    public float currentShake;

    [Header("References")]
    public GameObject cameraFocus;
    [FormerlySerializedAs("shakeGO")]
    public Camera cam;

    public void Update() {
        UpdateCamera();
        UpdateShake();
    }

    public void UpdateShake() {
        currentShake = currentShake.LerpTo(0, 20);
        cam.transform.localPosition = new Vector3(
            Random.Range(-currentShake, currentShake), 
            Random.Range(-currentShake, currentShake), 
            0);
    }

    public void UpdateCamera() {
        if (Unit.heroUnits.Count == 0 || Unit.monsterUnits.Count == 0) return;
		
        cameraFocus.transform.position = (Vector3.forward * -10) + Vector3.right *
            (Unit.heroUnits.GetHighest(unit => unit.GetX())
             + Unit.monsterUnits.GetLowest(unit => unit.GetX())) / 2;
			
        if (Mathf.Abs(cameraFocus.GetX() - this.GetX()) > maxDistFromUnits) 
            transform
                .LerpTo(cameraFocus, speed)
                .ClampX(-maxDistFromMapCenter, maxDistFromMapCenter);
    }

    public void Shake(float amount) {
        currentShake = amount;
    }
}
