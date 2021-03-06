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
        
    public bool isFull => slots.All(s => s.hero != null);

    //Walking is not an actual activity with slots, it's the filler between activities
    public enum Type { IDLE, SLEEPING, READY }
        

    public void Init() {
        button.onClick.AddListener(AddSelected);
        slots.ForEach(s => s.Init(this));
    }

    public void AddSelected() => Add(Camp.m.selectedHero, true);
    public void Add(CampHero campHero, bool deselect = false) {
        if (!IsOpenTo(campHero)) return;
        
        CampSlot heroSlot;
        if (type == Type.IDLE) heroSlot = slots[campHero.idleSlot];
        else heroSlot = slots.FirstOrDefault(s => s.hero == null);
        campHero.SetGoal(this, heroSlot);
        
        if (deselect) {
            Game.m.PlaySound(MedievalCombat.UI_TIGHT, .5f, 2);
            Camp.m.DeselectUnit();
        }
    }

    public bool IsOpenTo(CampHero hero) {
        if (hero == null) return false;
        if (this == hero.currentActivity) return false;
        if (isFull) return false;
        if (type == Type.READY && hero.data.currentHealth == 0) return false;
        if (type == Type.SLEEPING && hero.data.currentHealth.isAbout(hero.data.maxHealth)) return false;
        
        return true;
    }

    public void BounceButton() {
        button.Bounce(0.05f, .1f);
        button.GetComponent<Image>().TweenAlpha(0, Tween.Style.EASE_IN, .35f,
            () => {
                button.gameObject.SetActive(false);
                button.GetComponent<Image>().SetAlpha(1);
            });
    }
}