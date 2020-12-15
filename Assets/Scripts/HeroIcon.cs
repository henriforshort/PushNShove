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

    public enum UltAnim { AVAILABLE, LOADING, SHINE, USED }

    public void Init(Hero h) {
        icon.sprite = h.image;
        hero = h;
        h.icon = this;
    }

    public void Update() {
        UpdateUltTimer();
    }

    public void SetTmpHealth(float health) {
        tmpHealthBar.value = health;
    }

    public void FlashHealth() {
        healthBar.gameObject.SetActive(false);
        this.Wait(0.1f, () => healthBar.gameObject.SetActive(true));
    }

    public void SetHealth(float health) {
        healthBar.value = health;
        tmpHealthBar.value = health;
    }

    public void UpdateUltTimer() {
        if (ultCooldown.fillAmount == 0) return;
        
        if (ultCooldown.fillAmount > 0) ultCooldown.fillAmount -= Time.deltaTime / hero.ultCooldown;
        if (ultCooldown.fillAmount == 0)  PlayUltAnim(UltAnim.SHINE);
    }

    public void PlayUltAnim(UltAnim ba) {
        if (ultAnim == ba) return;
        
        ultAnim = ba;
        backgroundAnimator.Play(ultAnim.ToString());
    }

    public void Ult() {
        if (ultCooldown.fillAmount > 0) return;
        hero.Ult();
        PlayUltAnim(UltAnim.USED);
        this.Wait(1, () => {
            ultCooldown.fillAmount = 1;
            PlayUltAnim(UltAnim.LOADING);
        });
    }
}