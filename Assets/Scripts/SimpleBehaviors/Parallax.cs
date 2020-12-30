using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [Range(0,1)] public float amount;
    private float startX;

    private void Start() {
        startX = this.GetX();
    }

    private void Update() {
        this.SetX(startX + Battle.m.cameraManager.GetX() * amount);
    }
}