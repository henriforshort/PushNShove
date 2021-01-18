﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CampActivity {
    public Type type;
    public Button button;
    public List<GameObject> fullMarkers;
    public List<CampSlot> slots;
        
    public CampSlot emptySlot => slots.FirstOrDefault(s => s.hero == null);

    //Walking is not an actual activity with slots, it's the filler between activities
    public enum Type { IDLE, SLEEPING, READY, WALKING }
        

    public void Init() => button.onClick.AddListener(AddSelected);

    public void AddSelected() {
        if (Add(Camp.m.selectedHero)) Camp.m.DeselectUnit();
    }
        
    public bool Add(CampHero campHero) {
        bool open = this.IsOpenTo(campHero);
        if (open) campHero.SetGoal(this, emptySlot);
        return open;
    }

    public bool IsOpenTo(CampHero hero) {
        if (hero == null) return false;
        if (this == hero.currentActivity) return false;
        if (emptySlot == default) return false;
        if (type == Type.READY && hero.data.currentHealth == 0) return false;
        if (type == Type.SLEEPING && hero.data.currentHealth.isAbout(hero.data.maxHealth)) return false;
        return true;
    }
}