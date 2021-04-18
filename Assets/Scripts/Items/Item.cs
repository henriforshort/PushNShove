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
    public UnitHero hero;

    public static Item describedItem;
    
    public enum Rarity { COMMON, RARE, LEGGY }

    
    // ====================
    // EFFECT
    // ====================

    public void Init(Item prefab, UnitHero hero, bool fromFight) {
        this.prefab = prefab;
        this.hero = hero;
        name = this.prefab.name;
        description = name+"\n"+description;
        effect.item = this;
        if (fromFight) {
            this.hero.itemPrefabPaths.Add(this.prefab.ToPath("Items/"));
            effect.Apply();
        }
    }

    public void SwitchTo(UnitHero newHero) {
        if (hero == newHero) return;

        Game.m.PlaySound(MedievalCombat.COIN_AND_PURSE, .5f, -1, SoundManager.Pitch.RANDOM);
        effect.Cancel();
        hero.itemPrefabPaths.Remove(prefab.ToPath("Items/"));
        hero = newHero;
        hero.itemPrefabPaths.Add(prefab.ToPath("Items/"));
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
        if (hero.itemPrefabPaths.Count >= Game.m.maxItemsPerHero) hero.icon.itemPanel.FlashRed();
        else if (Run.m.movingItem.hero.itemPrefabPaths.Count >= Game.m.maxItemsPerHero) 
            Run.m.movingItem.hero.icon.itemPanel.FlashRed();
        else {
            UnitHero otherhero = Run.m.movingItem.hero;
            Run.m.movingItem.SwitchTo(hero);
            SwitchTo(otherhero);
        }
    }
}