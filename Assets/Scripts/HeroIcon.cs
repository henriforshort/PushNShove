using UnityEngine;
using UnityEngine.UI;

public class HeroIcon : MonoBehaviour {
    [Header("Self References")]
    public Image icon;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Image ultCooldown;

    [Header("Scene References (Assigned at runtime)")]
    public Hero hero;

    public void Init(Hero h) {
        icon.sprite = h.image;
        hero = h;
        h.icon = this;
    }

    public void Update() {
        if (tmpHealthBar.value.isClearlyNot(healthBar.value)) tmpHealthBar.LerpTo(healthBar.value, 3f);
        if (ultCooldown.fillAmount > 0) ultCooldown.fillAmount -= Time.deltaTime / hero.ultCooldown;
    }

    public void SetTmpHealth(float health) {
        tmpHealthBar.value = health;
    }

    public void SetHealth(float health) {
        healthBar.value = health;
    }

    public void Ult() {
        if (ultCooldown.fillAmount > 0) return;
        hero.Ult();
        ultCooldown.fillAmount = 1;
    }
}