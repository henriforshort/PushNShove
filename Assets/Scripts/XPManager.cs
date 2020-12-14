using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour {
    [Header("References")]
    public TMP_Text levelText;
    public GameObject lvUpNotif;
    public GameObject lvUpMenu;
    public Slider xpSlider;
    public Animator xpAnimator;
    public TMP_Text xpText;
    public TMP_Text levelUpText;

    public void Start() {
        lvUpNotif.SetActive(R.m.save.skillPoints > 0);
        xpSlider.value = R.m.save.experience; //Set xp bar with no lerp
        levelText.text = R.m.save.level.ToString();
    }

    public void Update() {
        UpdateXp();
    }
    
    public void GainExperience(float amount) {
        R.m.save.experience += amount;
        if (R.m.save.experience >= 1) LevelUp();
        this.Wait(1, () => xpAnimator.SetInteger("shine", 1));
        this.Wait(2, () => xpAnimator.SetInteger("shine", 0));
    }

    public void UpdateXp() {
        if (xpSlider.value.isAbout(R.m.save.experience)) xpSlider.value = R.m.save.experience;
        else xpSlider.value = xpSlider.value.LerpTo(R.m.save.experience, 2);
        
        xpText.text = (100 * xpSlider.value).Round() + "/100";
    }

    public void LevelUp() {
        R.m.save.experience--;
        xpSlider.value = 0;
        
        R.m.save.level++;
        levelText.text = R.m.save.level.ToString();
        levelUpText.gameObject.SetActive(true);
        this.Wait(1, () => levelUpText.gameObject.SetActive(false));

        R.m.save.skillPoints++;
        lvUpNotif.SetActive(true);
    }

    public void OpenLvUpMenu() {
        lvUpMenu.SetActive(true);
    }
}