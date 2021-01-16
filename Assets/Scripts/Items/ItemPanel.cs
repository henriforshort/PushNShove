using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour, IDropHandler {
    public Hero hero;
    public Image image;
    
    public void OnDrop(PointerEventData eventData) { //What to do when drag ends on an icon
        if (hero.itemPrefabs.Count < Game.m.maxItemsPerHero) Run.m.movingItem.SwitchTo(hero);
        else FlashRed();
    }

    public void FlashRed() {
        image.SetAlpha(1f);
        image.TweenAlpha(0, Tween.Style.EASE_OUT, 1f);
    }
}