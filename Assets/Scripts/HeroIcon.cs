using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroIcon : MonoBehaviour {
    [Header("State")]
    public IconAnim iconAnim;
    
    [Header("Self References")]
    public Image icon;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Image ultCooldown;
    public Animator backgroundAnimator;
    public Image deadOverlay;

    [Header("Scene References (Assigned at runtime)")]
    public Hero hero;

    public enum IconAnim { LOADING, SHINE, USED, DEAD }
    
    
    // ====================
    // BASICS
    // ====================
    
    public void Update() {
        UpdateUltTimer();
    }
    
    public void PlayIconAnim(IconAnim ia) {
        if (iconAnim == ia) return;
        
        iconAnim = ia;
        backgroundAnimator.Play(iconAnim.ToString());
    }
    
    
    // ====================
    // HEALTH
    // ====================

    public void FlashHealth() {
        healthBar.gameObject.SetActive(false);
        this.Wait(0.1f, () => healthBar.gameObject.SetActive(true));
    }

    public void SetHealth(float health) {
        healthBar.value = health;
        tmpHealthBar.value = health;
    }
    
    
    // ====================
    // DEATH
    // ====================

    public void Die() {
        deadOverlay.gameObject.SetActive(true);
        PlayIconAnim(IconAnim.DEAD);
        ultCooldown.gameObject.SetActive(false);
    }
    
    
    // ====================
    // ULT
    // ====================

    public void UpdateUltTimer() {
        if (hero == null) return;
        ultCooldown.fillAmount = (hero.ultCooldownLeft / hero.ultCooldown).Clamp01();
    }

    public void Ult() {
        if (!hero.CanUlt()) return;

        hero.Ult();
        PlayIconAnim(IconAnim.USED);
    }

    public void StartUltReload() => PlayIconAnim(IconAnim.LOADING);
    public void ReadyUlt() => PlayIconAnim(IconAnim.SHINE);
}