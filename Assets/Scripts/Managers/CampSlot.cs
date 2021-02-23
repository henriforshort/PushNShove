using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CampSlot {
    public CampHero hero;
    public float x;
    public Button button;
    public List<GameObject> emptyMarkers;
    [NonSerialized] public CampActivity activity;

    public void Init(CampActivity a) {
        button.onClick.AddListener(OnClick);
        activity = a;
    }

    public void OnClick() {
        if (hero == null) activity.AddSelected();
        else if (CanBeClicked(hero)) Camp.m.SelectUnit(hero);
    }

    public bool CanBeClicked(CampHero h) {
        if (h.data.activity != CampActivity.Type.SLEEPING) return true;
        if (h.data.currentHealth > 0.1f) return true;
        
        return false;
    }
}