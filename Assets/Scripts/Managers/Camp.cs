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
    public CampActivity idle;
    public CampActivity sleep;
    public CampActivity ready;
    
    public List<CampActivity> activities => new List<CampActivity> { idle, sleep, ready };

    public void Start() {
        InitCamp();
    }

    public void InitCamp() {
        this.While(() => true, () => Game.m.PlaySound(MedievalCombat.MAGIC_LOOP_FIRE, .25f), 5f);
            
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
        Game.m.PlaySound(MedievalCombat.SPECIAL_CLICK, 1, 3);
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
        heroes.ForEach(h => h.data.activity = h.currentActivity.type);
    }

    public CampActivity GetActivity(CampActivity.Type wantedType) 
        => activities.FirstOrDefault(a => a.type == wantedType);
    
    public void SelectUnit(CampHero campHero) {
        Game.m.PlaySound(MedievalCombat.UI_TIGHT, 1, 1);
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

        CampActivity selectedActivity = selectedHero.currentActivity; //stays the same after the end of the method
        CampHero oldSelectedHero = selectedHero;
        activities.Except(selectedActivity).ForEach(a => a.button.gameObject.SetActive(false));
        selectedHero = null;
        if (oldSelectedHero.data.activity != CampActivity.Type.WALKING) {
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

    public void HealAllHeroes() => heroes.ForEach(h => h.AddHealth(20));
    public void HurtAllHeroes() => heroes.ForEach(h => h.AddHealth(-20));
}
