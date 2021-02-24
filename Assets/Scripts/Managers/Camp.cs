using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Camp : Level<Camp> {
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
    
    
    // ====================
    // BASICS
    // ====================

    public void Start() {
        InitCamp();
    }
    
    
    // ====================
    // CAMP INIT
    // ====================

    public void InitCamp() {
        //Play music and ambient loops
        Game.m.PlayMusic(AdventureRPG.OUR_VILLAGE);
        this.While(() => true, 
            () => Game.m.PlaySound(MedievalCombat.MAGIC_LOOP_FIRE, .05f), 5f);
        this.While(() => true, 
            () => Game.m.PlaySound(Animals.OWL_3, .25f), 10f);
            
        //Create heroes
        Game.m.save.heroes.ForEach(hgs => {
            CampHero newHero = Instantiate(hgs.campPrefab, unitsHolder);
            heroes.Add(newHero);
            newHero.prefabIndex = hgs.prefabIndex;
        });
        
        //Init activities
        DeselectUnit();
        activities.ForEach(a => a.Init());
    }


    // ====================
    // ACTIVITIES
    // ====================
    
    public void SelectUnit(CampHero campHero) {
        Game.m.PlaySound(MedievalCombat.UI_TIGHT, 1, 1);
        arrow.locked = (selectedHero == null);
        selectedHero = campHero;
        arrow.spriteRenderer.enabled = true;
        activities.ForEach(a => {
            a.button.gameObject.SetActive(a.IsOpenTo(campHero));
        });
    }

    public void ClickOutside() => DeselectUnit(); //Called by ui button
    public void DeselectUnit() {
        arrow.spriteRenderer.enabled = false;
        if (selectedHero == null) return;

        CampActivity selectedActivity = selectedHero.currentActivity; //stays the same after the end of the method
        CampHero oldSelectedHero = selectedHero;
        activities.Except(selectedActivity).ForEach(a => a.button.gameObject.SetActive(false));
        selectedHero = null;
        if (oldSelectedHero.isWalking) {
            selectedActivity.button.gameObject.SetActive(false);
            return;
        }
        selectedActivity.BounceButton();
    }
    
    public CampActivity GetActivity(CampActivity.Type wantedType) 
        => activities.FirstOrDefault(a => a.type == wantedType);
    
    
    // ====================
    // START BATTLE
    // ====================
    
    public void StartBattle() { //Called by ui button
        Game.m.PlaySound(MedievalCombat.SPECIAL_CLICK, 1, 3);
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
        Game.m.SaveToDevice();
    }
    
    
    // ====================
    // CHEATS
    // ====================

    public void HealAllHeroes() => heroes.ForEach(h => h.AddHealth(20));
    public void HurtAllHeroes() => heroes.ForEach(h => h.AddHealth(-20));
}
