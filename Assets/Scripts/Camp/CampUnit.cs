using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampUnit : MonoBehaviour {
    [Header("State")]
    public Camp.Activity.Type status;
    public Camp.Slot currentSlot;
    [HideInInspector] public Camp.Activity currentActivity;
    
    [Header("References")]
    public GameObject idleVisuals;
    public GameObject sleepingVisuals;
    public GameObject walkingVisuals;
    public GameObject readyVisuals;

    public void Start() {
        Camp.m.activities.FirstOrDefault(a => a.type == status)?.Add(this);
        this.SetX(currentSlot.x);
    }

    public void Update() {
        Camp.Activity activity = Camp.m.activities.FirstOrDefault(a => a.type == status);
        if (activity != null && activity != currentActivity) activity.Add(this);
        
        if (Input.GetKeyDown(KeyCode.C)) currentActivity.Add(this);
        MoveToGoal();
    }

    public void MoveToGoal() {
        if (status != Camp.Activity.Type.WALKING) return;

        float dif = currentSlot.x - this.GetX();
        this.SetX(this.GetX() + dif.Sign() * (Time.deltaTime * 2f).AtMost(dif.Abs()));
        if (this.GetX().isAbout(currentSlot.x)) ReachGoal();
    }

    public void SetGoal(Camp.Activity newActivity, Camp.Slot newSlot) {
        if (currentSlot != null) {
            currentSlot.unit = null;
            currentSlot.emptyMarkers.ForEach(m => m.SetActive(true));
            currentActivity.fullMarkers.ForEach(m => m.SetActive(false));
        }
        currentSlot = newSlot;
        currentActivity = newActivity;
        currentSlot.unit = this;
        SetStatus(Camp.Activity.Type.WALKING);
        this.SetMirrored(currentSlot.x < this.GetX());
        this.SetZ(-2);
    }

    public void ReachGoal() {
        currentSlot.emptyMarkers.ForEach(m => m.SetActive(false));
        currentActivity.fullMarkers.ForEach(m => m.SetActive(currentActivity.emptySlot == default));
        SetStatus(currentActivity.type);
        this.SetX(currentSlot.x);
        if (status == Camp.Activity.Type.IDLE) {
            this.SetZ(-0.1f*currentSlot.x.Abs());
            this.SetMirrored(currentSlot.x > 0);
        }
        if (status == Camp.Activity.Type.SLEEPING) {
            this.SetZ(-2 - 0.1f*currentSlot.x);
            this.SetMirrored(false);
        }
        if (status == Camp.Activity.Type.READY) {
            this.SetZ(0);
            this.SetMirrored(false);
        }
    }

    public void SetStatus(Camp.Activity.Type newStatus) {
        status = newStatus;
        List<GameObject> visuals = new List<GameObject>{ idleVisuals, sleepingVisuals, readyVisuals, walkingVisuals };
        visuals.ForEach(s => s.SetActive(false));
        visuals[(int)status].SetActive(true);
        visuals[(int)status].GetComponent<Hanimator>()?.Play(0);
    }
}
