using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPanel : MonoBehaviour, IDropHandler {
    public Hero hero;
    
    public void OnDrop(PointerEventData eventData) { //What to do when drag ends on an icon
        Debug.Log("drag to "+name);
        if (!isFull) B.m.movingItem.SwitchTo(hero);
    }

    public void toto() => Debug.Log("toto");

    public bool isFull => hero.items.Count > R.m.maxItemsPerHero;
}