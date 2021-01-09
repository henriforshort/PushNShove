using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Camp : Level<Camp> {
    public UITransition transition;
    public Activity sleep;
    public Activity idle;
    public Activity ready;
    
    public void StartBattle() {
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
    }

    [Serializable]
    public class Slot {
        public CampUnit campUnit;
        public float x;
    }

    [Serializable]
    public class Activity {
        public string name;
        public int amountOfSlots;
        public List<Slot> slots;

        public Activity() {
            slots = new List<Slot>(amountOfSlots);
        }

        public void Add(CampUnit campUnit) {
            if (slots.Count < amountOfSlots) {
                Slot slot = slots[slots.IndexOf(null)];
                slot.campUnit = campUnit;
                campUnit.SetX(slot.x);
            } else {
                Debug.Log("activity is full");
            }
        }

        public void Remove(CampUnit campUnit) {
            if (slots.Select(s => s.campUnit).Contains(campUnit)) {
                slots.Remove(slots.FirstOrDefault(s => s.campUnit = campUnit));
            } else {
                Debug.Log(name + " does not contain "+campUnit.name);
            }
        }
    }
}
