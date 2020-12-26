using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    [Header("Balancing")]
    public Rarity rarity;
    [TextArea] public string description;
    
    [Header("State")]
    public bool isDragged;
    
    [Header("Self References")]
    public CanvasGroup canvasGroup;
    
    [Header("Scene References (assigned at runtime)")]
    public Hero hero;
    public Item prefab;

    public static Item describedItem;
    
    public enum Rarity { COMMON, RARE, LEGGY }

    public void Init(Item p, Hero h) {
        prefab = p;
        hero = h;
        description = name+description;
    }

    public void Update() {
        if (isDragged) this.LerpTo(Input.mousePosition, 20);
    }

    public void ApplyEffect() {
        // Debug.Log("apply effect of " + name + " to " + hero.name);
    }

    public void RemoveEffect() {
        // Debug.Log("remove effect of " + name + " from " + hero.name);
    }

    public void SwitchTo(Hero h) {
        if (hero == h) return;
        
        RemoveEffect();
        hero = h;
        ApplyEffect();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (B.m.movingItem != null) return;
        if (B.m.gameState != B.State.PLAYING) return;
        
        B.m.movingItem = this;
        transform.SetParent(B.m.itemsCanvas);
        canvasGroup.blocksRaycasts = false;
        isDragged = true;
        B.m.itemDescription.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData) { //What to do when drag ends, either on an icon or not
        if (B.m.movingItem != this) return;
        if (B.m.gameState != B.State.PLAYING) return;
        
        B.m.movingItem = null;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(hero.icon.itemPanel.transform);
        isDragged = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (B.m.itemDescription.activeSelf && describedItem == this) {
            B.m.itemDescription.SetActive(false);
        }
        else {
            B.m.itemDescription.transform.position = transform.position;
            B.m.itemDescription.SetActive(true);
            B.m.itemDescriptionText.text = description;
            describedItem = this;
        }
    }
}