using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour {
    [Header("Balancing")]
    public float hillsParallax;
    
    [Header("State")]
    public float currentShake;

    [Header("References")]
    public GameObject cameraFocus;
    [FormerlySerializedAs("shakeGO")]
    public GameObject cameraGO;
    public GameObject hills;
    public GameObject sun;

    public void Update() {
        if (B.m.gameState == B.State.PLAYING) {
            UpdateCameraAndParallax();
        }
        
        UpdateShake();
    }

    public void UpdateShake() {
        currentShake = currentShake.LerpTo(0, 20);
        cameraGO.transform.localPosition = new Vector3(
            Random.Range(-currentShake, currentShake), 
            Random.Range(-currentShake, currentShake), 
            0);
    }

    public void UpdateCameraAndParallax() {
        if (Unit.heroUnits.Count == 0 || Unit.monsterUnits.Count == 0) return;
		
        cameraFocus.transform.position = (Vector3.forward * -10) + Vector3.right *
            (Unit.heroUnits.Select(unit => unit.GetX()).Max()
             + Unit.monsterUnits.Select(unit => unit.GetX()).Min()) / 2;
			
        if (Mathf.Abs(cameraFocus.GetX() - this.GetX()) > R.m.camMaxDistFromUnits) 
            transform
                .LerpTo(cameraFocus, R.m.camSpeed)
                .ClampX(-R.m.camMaxDistFromMapCenter, R.m.camMaxDistFromMapCenter);

        hills.SetX(this.GetX() * hillsParallax);
        sun.SetX(this.GetX());
    }

    public void Shake(float amount) {
        currentShake = amount;
    }
}
