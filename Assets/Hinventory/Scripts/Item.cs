using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hiloqo.Hinventory {
    public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("Assign this (create your own item types!)")]
        public Type type;
        public int amount;
        public Slot currentSlot;
        
        [Header("Ignore this")]
        public Graphic graphic;
        public TMP_Text amountText;
        
        public enum Type { BONE, HOURGLASS, CARROT } //Write your own item types here

        public static Item draggedItem;

        public void Awake() {
            currentSlot.currentItem = this;
        }

        public ItemType GetItemType() {
            ItemType itemType = Hinventory.i.resources.itemTypes.First(it => it.type == type);
            if (itemType == null) Debug.LogWarning("Type "+type+" hasnt been setup. Please set up item types " +
                                                   "in Hinventory/Resources");
            return itemType;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (draggedItem != null) return;
            if (!CanBeDragged()) return;

            graphic.raycastTarget = false;
            draggedItem = this;
            currentSlot.currentItem = null;
            transform.SetParent(Hinventory.i.draggedItemPanel);
        }

        public void OnDrag(PointerEventData eventData) {
            transform.position = Input.mousePosition; 
        }

        public void OnEndDrag(PointerEventData eventData) {
            graphic.raycastTarget = true;
            if (draggedItem == this) draggedItem = null;
            transform.position = currentSlot.transform.position;
            currentSlot.currentItem = this;
            AfterDropped();
        }

        public bool CanBeDragged() {
            //Write here the conditions that allow an item to be dragged
            return true;
        }

        public void AfterDropped() {
            //Write here what happens when an item is dropped to a new slot
        }
    }
}