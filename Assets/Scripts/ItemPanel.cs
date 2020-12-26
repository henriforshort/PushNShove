using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPanel : MonoBehaviour, IDropHandler {
    public Hero hero;
    
    public void OnDrop(PointerEventData eventData) { //What to do when drag ends on an icon
        if (!isFull) B.m.movingItem.SwitchTo(hero);
    }

    public bool isFull => hero.items.Count > R.m.maxItemsPerHero;
}