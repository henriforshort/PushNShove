using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Camp : Level<Camp> {
    [Header("References")]
    public UITransition transition;
    public Activity idle;
    public Activity sleep;
    public Activity ready;
    
    public List<Activity> activities => new List<Activity> { idle, sleep, ready };
    
    public void StartBattle() {
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
    }

    [Serializable]
    public class Slot {
        public CampUnit unit;
        public float x;
    }

    [Serializable]
    public class Activity {
        public Type type;
        public List<Slot> slots;
        
        //Walking is not an actual activity with slots, it's the filler between activities
        public enum Type { IDLE, SLEEPING, READY, WALKING } 
        
        public void Add(CampUnit campUnit) {
            Slot emptySlot = slots.FirstOrDefault(s => s.unit == null);
            if (emptySlot == default) {
                Debug.Log("activity is full");
                return;
            }

            Activity oldActivity = campUnit.currentActivity;
            campUnit.SetGoal(this, emptySlot);

            if (oldActivity.type == Type.IDLE) {
                List <Slot> ls = oldActivity.slots.Clone();
                int security = 0;
                while (ls.Count > 0) {
                    if (ls[0].unit == null && ls.Count >= 3 && ls[2].unit != null) {
                        Camp.m.Wait(0.25f, () => oldActivity.Add(ls[2].unit));
                        break;
                    } else ls.RemoveAt(0);
                    
                    security++;
                    if (security > 100) {
                        Debug.Log("STOP");
                        ls.Select(s => s.unit != null).ToList().Log();
                        break;
                    }
                }
            }
        }
    }
}
