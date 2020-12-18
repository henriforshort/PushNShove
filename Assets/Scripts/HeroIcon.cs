using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroIcon : MonoBehaviour {
    [Header("State")]
    public UltAnim ultAnim;
    
    [Header("Self References")]
    public Image icon;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Image ultCooldown;
    public Animator backgroundAnimator;

    [Header("Scene References (Assigned at runtime)")]
    public Hero hero;

    public enum UltAnim { LOADING, SHINE, USED }
    
    
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
    // ULT
    // ====================

    public void Update() {
        UpdateUltTimer();
    }

    public void UpdateUltTimer() {
        if (hero == null) return;
        ultCooldown.fillAmount = (hero.ultCooldownLeft / hero.ultCooldown).Clamp01();
    }

    public void Ult() {
        if (!hero.CanUlt()) return;

        hero.Ult();
        PlayUltAnim(UltAnim.USED);
    }
    
    public void PlayUltAnim(UltAnim ba) {
        if (ultAnim == ba) return;
        
        ultAnim = ba;
        backgroundAnimator.Play(ultAnim.ToString());
    }

    public void StartUltReload() => PlayUltAnim(UltAnim.LOADING);
    public void ReadyUlt() => PlayUltAnim(UltAnim.SHINE);
}