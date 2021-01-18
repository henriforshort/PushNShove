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

    public void Init() {
        button.onClick.AddListener(() => {
            if (hero == null) activity.AddSelected();
            else if (hero.CanBeClicked()) Camp.m.SelectUnit(hero);
        });
    }
}