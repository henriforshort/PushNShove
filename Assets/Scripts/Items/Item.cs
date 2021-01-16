using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, 
    IDropHandler {
    [Header("Balancing")]
    public Rarity rarity;
    [TextArea] public string description;
    
    [Header("State")]
    public bool isDragged;
    
    [Header("Self References")]
    public CanvasGroup canvasGroup;
    public ItemEffect effect;
    public Item prefab;
    
    [Header("Scene References (assigned at runtime)")]
    public Hero hero;

    public static Item describedItem;
    
    public enum Rarity { COMMON, RARE, LEGGY }

    
    // ====================
    // EFFECT
    // ====================

    public void Init(Item prefab, Hero hero, bool fromFight) {
        this.prefab = prefab;
        this.hero = hero;
        name = this.prefab.name;
        description = name+"\n"+description;
        effect.item = this;
        if (fromFight) {
            this.hero.itemPrefabs.Add(this.prefab);
            effect.Apply();
        }
    }

    public void SwitchTo(Hero newHero) {
        if (hero == newHero) return;

        effect.Cancel();
        hero.itemPrefabs.Remove(prefab);
        hero = newHero;
        hero.itemPrefabs.Add(prefab);
        effect.Apply();
    }
    
    
    // ====================
    // DRAG
    // ====================

    public void OnBeginDrag(PointerEventData eventData) {
        if (Run.m.movingItem != null) return;
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        
        Run.m.movingItem = this;
        transform.SetParent(Battle.m.itemsCanvas);
        canvasGroup.blocksRaycasts = false;
        isDragged = true;
        Battle.m.itemDescription.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData) => transform.position = Input.mousePosition;

    public void OnEndDrag(PointerEventData eventData) { //What to do when drag ends, either on an icon or not
        if (Run.m.movingItem != this) return;
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        
        Run.m.movingItem = null;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(hero.icon.itemPanel.transform);
        isDragged = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (Battle.m.itemDescription.activeSelf && describedItem == this) {
            Battle.m.itemDescription.SetActive(false);
        }
        else {
            Battle.m.itemDescription.transform.position = transform.position;
            Battle.m.itemDescription.SetActive(true);
            Battle.m.itemDescriptionText.text = description;
            describedItem = this;
        }
    }
    
    public void OnDrop(PointerEventData eventData) { //Not active for now. Look into UI layers
        if (hero.itemPrefabs.Count >= Game.m.maxItemsPerHero) hero.icon.itemPanel.FlashRed();
        else if (Run.m.movingItem.hero.itemPrefabs.Count >= Game.m.maxItemsPerHero) 
            Run.m.movingItem.hero.icon.itemPanel.FlashRed();
        else {
            Hero otherhero = Run.m.movingItem.hero;
            Run.m.movingItem.SwitchTo(hero);
            SwitchTo(otherhero);
        }
    }
}