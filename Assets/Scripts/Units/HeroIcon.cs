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
    public ItemPanel itemPanel;

    [Header("Scene References (Assigned at runtime)")]
    public Hero hero;

    public enum IconAnim { LOADING, SHINE, USED, DEAD }
    
    
    // ====================
    // BASICS
    // ====================

    public void InitBattle(Hero h) {
        hero = h;
        icon.sprite = h.image;
        itemPanel.hero = h;
    }
    
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
        SetHealth(0);
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
    
    
    // ====================
    // ITEMS
    // ====================

    public void ClearItems() {
        foreach (Transform i in itemPanel.transform) Destroy(i.gameObject);
    }

    public void GainItemFromFight(Item itemPrefab, Vector3 position) {
        //Create item
        Item itemInstance = Instantiate(
            itemPrefab, 
            Battle.m.cameraManager.cam.WorldToScreenPoint(position),
            Quaternion.identity,
            Battle.m.itemsCanvas);
        itemInstance.Init(itemPrefab, hero, true);
        
        //Bounce item then move it to top left corner
        GameObject placeholder = new GameObject();
        placeholder.AddComponent<Image>().sprite = Game.m.transparentSprite;
        placeholder.transform.SetParent(itemPanel.transform);
        itemInstance.TweenPosition(itemInstance.transform.position + Vector3.up * 50, 
            Tween.Style.BOUNCE, 0.5f, () => 
                this.Wait(0.25f, () => 
                    itemInstance.TweenPosition(placeholder.transform.position, Tween.Style.EASE_OUT, 0.5f, 
                        () => {
                        Destroy(placeholder);
                        itemInstance.transform.SetParent(itemPanel.transform);
                        Game.m.PlaySound(MedievalCombat.COIN_AND_PURSE);
                    })));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        Item itemInstance = Instantiate(itemPrefab, itemPanel.transform);
        itemInstance.Init(itemPrefab, hero, false);
    }
}