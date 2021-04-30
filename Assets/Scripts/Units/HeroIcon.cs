using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroIcon : MonoBehaviour {
    [Header("State")]
    public IconAnim iconAnim;
    
    [Header("Self References")]
    public Image icon;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Image ultCooldown;
    public Hanimator iconFxAnimator;
    public Image deadOverlay;
    public ItemPanel itemPanel;
    public Slider xpBar;
    public TMP_Text levelNumber;
    public TMP_Text levelUpText;
    public GameObject doubleXp;

    [Header("Scene References (Assigned at runtime)")]
    public UnitHero hero;

    public enum IconAnim { LOADING, SHINE, USED, DEAD }
    
    
    // ====================
    // BASICS
    // ====================

    public void InitBattle(UnitHero h) {
        hero = h;
        icon.sprite = h.image;
        itemPanel.hero = h;
        levelUpText.SetAlpha(0);
        doubleXp.SetActive(hero.unit.data.isOnDoubleXp);
    }
    
    public void Update() {
        UpdateUltTimer();
    }
    
    public void PlayIconAnim(IconAnim ia) {
        if (iconAnim == ia) return;
        
        iconAnim = ia;
        iconFxAnimator.Play(iconAnim.ToString());
    }
    
    
    // ====================
    // HEALTH
    // ====================

    public void SetHealth(float health, float tempHealth = 0) {
        healthBar.enabled = false;
        this.Wait(0.1f, () => healthBar.enabled = true);
        
        healthBar.value = health;
        tmpHealthBar.value = tempHealth;
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
        Battle.m.Pause(1.5f);
        
        //Create item
        Item itemInstance = Instantiate(
            itemPrefab, 
            Battle.m.cameraManager.cam.WorldToScreenPoint(position),
            Quaternion.identity,
            Battle.m.itemsCanvas);
        itemInstance.transform.localScale = 2 * Vector3.one;
        itemInstance.Init(itemPrefab, hero, true);
        itemInstance.glow.SetActive(true);
        itemInstance.rotatingGlow.SetActive(true);
        
        //Bounce item then move it to top left corner
        GameObject placeholder = new GameObject();
        placeholder.AddComponent<Image>().sprite = Game.m.transparentSprite;
        placeholder.transform.SetParent(itemPanel.transform);
        itemInstance.TweenPosition(itemInstance.transform.position + Vector3.up * 50, 
            Tween.Style.BOUNCE, 0.5f, () => 
                this.Wait(1f, () => {
                itemInstance.glow.SetActive(false);
                itemInstance.rotatingGlow.SetActive(false);
                itemInstance.TweenScale(Vector3.one, Tween.Style.EASE_OUT, 2f);
                itemInstance.TweenPosition(placeholder.transform.position, Tween.Style.EASE_OUT, 2f,
                    () => {
                        Destroy(placeholder);
                        itemInstance.transform.SetParent(itemPanel.transform);
                        this.Wait(.25f, () => itemInstance.Bounce(.1f));
                        Game.m.PlaySound(MedievalCombat.COIN_AND_PURSE, .5f, -1, SoundManager.Pitch.RANDOM);
                    });
            }));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        Item itemInstance = Instantiate(itemPrefab, itemPanel.transform);
        itemInstance.Init(itemPrefab, hero, false);
    }
    
    
    // ====================
    // XP
    // ====================

    public void GainXpFromFight(GameObject xpPrefab, Vector3 position, float amount) {
        //Create item
        GameObject xpInstance = Instantiate(xpPrefab, 
            Battle.m.cameraManager.cam.WorldToScreenPoint(position),
            Quaternion.identity,
            Battle.m.itemsCanvas);
        
        //Bounce item then move it to top left corner
        xpInstance.TweenPosition(Vector3.up * 50, 
            Tween.Style.BOUNCE, .5f, () => 
                this.Wait(0.25f, () => 
                    xpInstance.TweenPosition(ultCooldown.transform.position - xpInstance.transform.position, 
                        Tween.Style.EASE_OUT, 1f, () => {
                            Destroy(xpInstance);
                            if (.3f.Chance()) Game.m.PlaySound(MedievalCombat.COIN_AND_PURSE, 
                                .5f, -1, SoundManager.Pitch.RANDOM);
                            hero.unit.AddXp(amount);
                            hero.predictiveXp -= amount;
                        })));
    }
}