using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    public Hero hero;
    public Rarity rarity;
    public Item prefab;
    public Hero oldHero;
    public bool isDragged;
    public CanvasGroup canvasGroup;
    
    public enum Rarity { COMMON, RARE, LEGGY }

    public void Init(Item p, Hero h) {
        prefab = p;
        hero = h;
    }

    public void Update() {
        if (isDragged) transform.position = Vector3.Lerp(transform.position,Input.mousePosition, 2);
    }

    public void ApplyEffect() {
        Debug.Log("apply effect of " + name + " to " + hero.name);
    }

    public void RemoveEffect() {
        Debug.Log("remove effect of " + name + " from " + hero.name);
    }

    public void SwitchTo(Hero h) {
        Debug.Log(name+" switch");
        // RemoveEffect();
        hero = h;
        // ApplyEffect();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (B.m.movingItem != null) return;
        if (B.m.gameState != B.State.PLAYING) return;
        
        B.m.movingItem = this;
        transform.SetParent(B.m.itemsCanvas);
        canvasGroup.blocksRaycasts = false;
        oldHero = hero;
        isDragged = true;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData) { //What to do when drag ends, either on an icon or not
        if (B.m.movingItem != this) return;
        if (B.m.gameState != B.State.PLAYING) return;
        
        Debug.Log("dragEnd "+(oldHero == hero?"on same icon":"on a different icon"));
        B.m.movingItem = null;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(hero.icon.itemPanel.transform);
        isDragged = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        //Show tooltip
        // Debug.Log("click on "+name);
    }
}