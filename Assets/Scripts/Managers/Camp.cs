using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Camp : Level<Camp> {
    [Header("State")]
    public CampUnit selectedUnit;
    
    [Header("References")]
    public UITransition transition;
    public CampArrow arrow;
    
    [Header("Activities")]
    public Activity idle;
    public Activity sleep;
    public Activity ready;
    
    public List<Activity> activities => new List<Activity> { idle, sleep, ready };

    public void Start() {
        DeselectUnit();
        activities.ForEach(a => {
            a.Init();
            a.slots.ForEach(s => {
                s.activity = a;
                s.Init();
            });
        });
    }

    public void SelectUnit(CampUnit campUnit) {
        arrow.locked = (selectedUnit == null);
        selectedUnit = campUnit;
        arrow.spriteRenderer.enabled = true;
        activities.ForEach(a => 
            a.button.gameObject.SetActive(a != campUnit.currentActivity && a.emptySlot != default));
    }

    public void DeselectUnit() {
        arrow.spriteRenderer.enabled = false;
        if (selectedUnit == null) return;

        Activity selectedActivity = selectedUnit.currentActivity; //stays the same after the end of the method
        CampUnit oldSelectedUnit = selectedUnit;
        activities.Except(selectedActivity).ForEach(a => a.button.gameObject.SetActive(false));
        selectedUnit = null;
        if (oldSelectedUnit.status != Activity.Type.WALKING) {
            selectedActivity.button.gameObject.SetActive(false);
            return;
        }

        selectedActivity.button.SetNormalColor(Game.m.white);
        selectedActivity.button.Bounce(0.05f, .1f);
        selectedActivity.button.GetComponent<Image>().TweenAlpha(0, Tween.Style.EASE_IN, .35f,
            () => {
                selectedActivity.button.SetNormalColor(Game.m.white);
                selectedActivity.button.gameObject.SetActive(false);
                selectedActivity.button.GetComponent<Image>().SetAlpha(1);
            });
    }
    
    public void StartBattle() {
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
    }

    public void ClickOutside() {
        DeselectUnit();
    }

    [Serializable]
    public class Slot {
        public CampUnit unit;
        public float x;
        public Button button;
        public List<GameObject> emptyMarkers;
        [NonSerialized] public Activity activity;

        public void Init() {
            button.onClick.AddListener(() => {
                if (unit == null || unit.status == Activity.Type.WALKING) activity.AddSelected();
                else Camp.m.SelectUnit(unit);
            });
        }
    }

    [Serializable]
    public class Activity {
        public Type type;
        public Button button;
        public List<GameObject> fullMarkers;
        public List<Slot> slots;
        
        public Slot emptySlot => slots.FirstOrDefault(s => s.unit == null);

        //Walking is not an actual activity with slots, it's the filler between activities
        public enum Type { IDLE, SLEEPING, READY, WALKING }
        

        public void Init() {
            button.onClick.AddListener(AddSelected);
        }

        public void AddSelected() {
            if (Camp.m.selectedUnit != null && Camp.m.selectedUnit.status != type) {
                Add(Camp.m.selectedUnit);
                Camp.m.DeselectUnit();
            }
        }
        
        public void Add(CampUnit campUnit) {
            if (emptySlot == default) {
                Debug.Log(type+" is full, can't add "+campUnit.name);
                return;
            }

            Activity oldActivity = campUnit.currentActivity;
            campUnit.SetGoal(this, emptySlot);

            // BELOW : move units if there is an empty slot in front
            
            // if (oldActivity.type == Type.IDLE) {
            //     List <Slot> ls = oldActivity.slots.Clone();
            //     while (ls.Count > 0) {
            //         if (ls[0].unit == null && ls.Count >= 3 && ls[2].unit != null) {
            //             Camp.m.Wait(0.25f, () => oldActivity.Add(ls[2].unit));
            //             break;
            //         } else ls.RemoveAt(0);
            //     }
            // }
        }
    }
}
