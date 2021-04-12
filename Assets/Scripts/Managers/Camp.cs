using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Camp : Level<Camp> {
    [Header("Balancing")]
    public float cameraY;
    
    [Header("State")]
    public CampHero selectedHero;
    public List<CampHero> heroes;
    
    [Header("References")]
    public GameObject cameraGO;
    public UITransition transition;
    public CampArrow arrow;
    public Transform unitsHolder;
    public TMP_Text unitsReadyNumber;
    public Graphic gemsBg;
    public TMP_Text gemsText;
    public Graphic gemsIcon;
    
    [Header("Activities")]
    public CampActivity idle;
    public CampActivity sleep;
    public CampActivity ready;
    
    public List<CampActivity> activities => new List<CampActivity> { idle, sleep, ready };
    
    
    // ====================
    // BASICS
    // ====================

    public void Start() {
        if (Time.frameCount == 1) FirstInit();
        InitCamp();
    }
    
    
    // ====================
    // CAMP INIT
    // ====================

    //Called the first time this scene loads in a given session (displays title screen & the like)
    public void FirstInit() {
        Game.m.save.heroes
            .Where(h => h.data.activity == CampActivity.Type.READY)
            .ToList()
            .ForEach(h => h.data.activity = CampActivity.Type.IDLE);

        cameraGO.SetY(cameraY);
        gemsBg.SetAlpha(0);
        gemsIcon.SetAlpha(0);
        gemsText.SetAlpha(0);
    }

    //Called every time this scene loads
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
        transition.FadeOut();
        
        //Misc
        if (Game.m.save.lastAssignedDoubleXp != DateTime.Today) AssignDoubleXp();
        UpdateUnitsReadyNumber();
        gemsText.text = Game.m.save.gems.ToString();

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
    // DOUBLE XP
    // ====================

    public void AssignDoubleXp() {
        Game.m.save.lastAssignedDoubleXp = DateTime.Today;
        CampHero hero1 = LowestLevelCharacterAmong(heroes);
        CampHero hero2 = LowestLevelCharacterAmong(heroes.Except(hero1));
        CampHero hero3 = LowestLevelCharacterAmong(heroes.Except(hero1).Except(hero2));
        this.Random(hero1, hero2, hero3).data.DoubleXpForDays(Game.m.doubleXpDurationInHours / 24);
    }

    public CampHero LowestLevelCharacterAmong(List<CampHero> campHeroes) => campHeroes
            .Where(ch => ch.data.level == campHeroes.Select(ch2 => ch2.data.level).Min())
            .WithLowest(ch => ch.data.xp);


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
    // VISUALS
    // ====================

    public void TweenCamera() {
        if(cameraGO.GetY().isClearlyNot(cameraY)) return;
        cameraGO.TweenPosition(cameraY * Vector3.down, Tween.Style.EASE_OUT, 2);
        gemsBg.TweenAlpha(.25f, Tween.Style.EASE_OUT, 2);
        gemsIcon.TweenAlpha(1, Tween.Style.EASE_OUT, 2);
        gemsText.TweenAlpha(1, Tween.Style.EASE_OUT, 2);
    }

    public void UpdateUnitsReadyNumber() => unitsReadyNumber.text = 
        "[" + heroes.Count(h => h.currentActivity.type == CampActivity.Type.READY) + 
        "/" + Game.m.heroesPerBattle + "]";


    // ====================
    // CHEATS
    // ====================

    public void HealAllHeroes() => heroes.ForEach(h => h.AddHealth(20));
    public void HurtAllHeroes() => heroes.ForEach(h => h.AddHealth(-20));
}
