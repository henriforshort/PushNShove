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
    
    [Header("Scene References (assigned at runtime)")]
    public Hero hero;
    public Item prefab;

    public static Item describedItem;
    
    public enum Rarity { COMMON, RARE, LEGGY }

    public void Init(Item p, Hero h) {
        prefab = p;
        name = prefab.name;
        description = name+"\n"+description;
        effect.item = this;
        SwitchTo(h);
    }

    public void Update() {
        if (isDragged) this.LerpTo(Input.mousePosition, 20);
    }

    public void SwitchTo(Hero h) {
        if (hero == h) return;

        // Debug.Log(name + " switches to " + h.name);
        effect.Cancel();
        if (hero != null) hero.items.Remove(this);
        hero = h;
        hero.items.Add(this);
        effect.Apply();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (Run.m.movingItem != null) return;
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        
        Run.m.movingItem = this;
        transform.SetParent(Battle.m.itemsCanvas);
        canvasGroup.blocksRaycasts = false;
        isDragged = true;
        Battle.m.itemDescription.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData) { }

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
        if (hero.items.Count >= Run.m.maxItemsPerHero) hero.icon.itemPanel.FlashRed();
        else if (Run.m.movingItem.hero.items.Count >= Run.m.maxItemsPerHero) 
            Run.m.movingItem.hero.icon.itemPanel.FlashRed();
        else {
            Hero otherhero = Run.m.movingItem.hero;
            Run.m.movingItem.SwitchTo(hero);
            SwitchTo(otherhero);
        }
    }
}