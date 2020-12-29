using UnityEngine;
using UnityEngine.EventSystems;

namespace Hiloqo.Hinventory {
    public class Slot : MonoBehaviour, IDropHandler {
        [Header("Ignore this")]
        public Item currentItem;
        public Inventory inventory;
        
        public void OnDrop(PointerEventData eventData) {
            if (currentItem == null) Item.draggedItem.currentSlot = this;
            else {
                if (currentItem.type == Item.draggedItem.type) {
                    ItemType type = currentItem.GetItemType();
                    if (type.maximumStack >= currentItem.amount + Item.draggedItem.amount) {
                        currentItem.amount += Item.draggedItem.amount;
                        Destroy(Item.draggedItem.gameObject);
                    } else {
                        Item.draggedItem.amount += currentItem.amount - type.maximumStack;
                        currentItem.amount = type.maximumStack;
                    }
                } else {
                    currentItem.currentSlot = Item.draggedItem.currentSlot;
                    Item.draggedItem.currentSlot = this;
                    currentItem.OnEndDrag(default);
                }
            }
        }
    }
}
