using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Camp : Level<Camp> {
    [FormerlySerializedAs("selectedUnit")]
    [Header("State")]
    public CampHero selectedHero;
    public List<CampHero> heroes;
    
    [Header("References")]
    public UITransition transition;
    public CampArrow arrow;
    public Transform unitsHolder;
    
    [Header("Activities")]
    public Activity idle;
    public Activity sleep;
    public Activity ready;
    
    public List<Activity> activities => new List<Activity> { idle, sleep, ready };

    public void Start() {
        InitCamp();
    }

    public void InitCamp() {
        //Create heroes
        Game.m.save.heroes.ForEach(hgs => {
            CampHero newHero = Instantiate(hgs.campPrefab, unitsHolder);
            heroes.Add(newHero);
            newHero.prefabIndex = hgs.prefabIndex;

        });
        DeselectUnit();
        activities.ForEach(a => {
            a.Init();
            a.slots.ForEach(s => {
                s.activity = a;
                s.Init();
            });
        });
    }
    
    public void StartBattle() {
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
        heroes.ForEach(h => h.data.activity = h.currentActivity.type);
    }

    public Activity GetActivity(Activity.Type wantedType) 
        => activities.FirstOrDefault(a => a.type == wantedType);
    
    public void SelectUnit(CampHero campHero) {
        arrow.locked = (selectedHero == null);
        selectedHero = campHero;
        arrow.spriteRenderer.enabled = true;
        activities.ForEach(a => {
            a.button.gameObject.SetActive(a.IsOpenTo(campHero));
        });
    }

    public void DeselectUnit() {
        arrow.spriteRenderer.enabled = false;
        if (selectedHero == null) return;

        Activity selectedActivity = selectedHero.currentActivity; //stays the same after the end of the method
        CampHero oldSelectedHero = selectedHero;
        activities.Except(selectedActivity).ForEach(a => a.button.gameObject.SetActive(false));
        selectedHero = null;
        if (oldSelectedHero.data.activity != Activity.Type.WALKING) {
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

    public void ClickOutside() {
        DeselectUnit();
    }

    public void OnApplicationQuit() {
        Game.m.SaveToDevice();
    }

    [Serializable]
    public class Slot {
        public CampHero hero;
        public float x;
        public Button button;
        public List<GameObject> emptyMarkers;
        [NonSerialized] public Activity activity;

        public void Init() {
            button.onClick.AddListener(() => {
                if (hero == null || hero.data.activity == Activity.Type.WALKING) activity.AddSelected();
                else Camp.m.SelectUnit(hero);
            });
        }
    }

    [Serializable]
    public class Activity {
        public Type type;
        public Button button;
        public List<GameObject> fullMarkers;
        public List<Slot> slots;
        
        public Slot emptySlot => slots.FirstOrDefault(s => s.hero == null);

        //Walking is not an actual activity with slots, it's the filler between activities
        public enum Type { IDLE, SLEEPING, READY, WALKING }
        

        public void Init() {
            button.onClick.AddListener(AddSelected);
        }

        public void AddSelected() {
            if (Camp.m.selectedHero == null) return;
            if (Camp.m.selectedHero.data.activity == type) return;
            
            Add(Camp.m.selectedHero);
            Camp.m.DeselectUnit();
        }
        
        public void Add(CampHero campHero) {
            if (emptySlot == default) {
                Debug.Log(type+" is full, can't add "+campHero.name);
                return;
            }
            campHero.SetGoal(this, emptySlot);
        }

        public bool IsOpenTo(CampHero hero) {
            if (this == hero.currentActivity) return false;
            if (emptySlot == default) return false;
            if (type == Type.READY && hero.data.currentHealth == 0) return false;
            if (type == Type.SLEEPING && hero.data.currentHealth.isAbout(hero.data.maxHealth)) return false;
            return true;
        }
    }
}
