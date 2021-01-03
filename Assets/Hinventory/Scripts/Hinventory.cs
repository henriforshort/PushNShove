using UnityEditor;
using UnityEngine;

namespace Hiloqo.Hinventory {
    [ExecuteInEditMode]
    public class Hinventory : MonoBehaviour {
        [Header("Ignore this")]
        public Transform draggedItemPanel;
        public GameObject rowPrefab;
        public Slot slotPrefab;
        public Resources resources;

        private static Hinventory _i;
        public static Hinventory i => _i ?? FindObjectOfType<Hinventory>();

        public void Awake() {
            if (_i == null) _i = this;
            if (_i != this) DestroyImmediate(gameObject);
        }

        #if UNITY_EDITOR
        public void Update() {
            SnapItems();
        }
        
        [MenuItem("Tools/Hinventory/Generate Selected Inventories")]
        public static void GenerateSelectedInventories() {
            foreach (GameObject go in Selection.gameObjects) {
                Inventory inv = go.GetComponent<Inventory>();
                if (inv != null) CreateInventory(inv);
            }
        }

        [MenuItem("Tools/Hinventory/Destroy Selected Inventories")]
        public static void DestroySelectedInventories() {
            foreach (GameObject go in Selection.gameObjects) {
                Inventory inv = go.GetComponent<Inventory>();
                if (inv != null) DestroyInventory(inv);
            }
        }

        [MenuItem("Tools/Hinventory/Generate All Inventories")]
        public static void GenerateAllInventories() {
            foreach (Inventory inv in FindObjectsOfType<Inventory>()) CreateInventory(inv);
        }

        [MenuItem("Tools/Hinventory/Destroy All Inventories")]
        public static void DestroyAllInventories() {
            foreach (Inventory inv in FindObjectsOfType<Inventory>()) DestroyInventory(inv);
        }

        public static void CreateInventory(Inventory inv) {
            if (inv.size.x < 1 || inv.size.y < 1) {
                Debug.LogError("inventory size has to be at least 1");
                return;
            }
            if (inv.slotSize.x < 1 || inv.slotSize.y < 1) {
                Debug.LogError("inventory slot size has to be at least 1");
                return;
            }
            DestroyInventory(inv);
            float totalWidth = inv.slotSize.x * inv.size.x + inv.spacing * (inv.size.x - 1);
            float totalHeight = inv.slotSize.y * inv.size.y + inv.spacing * (inv.size.y - 1);
            inv.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, totalHeight);
            // SetAnchors(inv.GetComponent<RectTransform>());
            for (int j = 0; j < inv.size.y; j++) {
                GameObject rgo = PrefabUtility.InstantiatePrefab(i.rowPrefab, inv.slotsHolder) as GameObject;
                rgo.name = "Row " + j;
                rgo.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, inv.slotSize.y);
                for (int k = 0; k < inv.size.x; k++) {
                    Slot sgo = PrefabUtility.InstantiatePrefab(i.slotPrefab, inv.slotsHolder.GetChild(j)) as Slot;
                    sgo.name = "Row " + j + " Slot " + k;
                    sgo.inventory = inv;
                    sgo.GetComponent<RectTransform>().sizeDelta = inv.slotSize;
                }
            }
        }

        public static void DestroyInventory(Inventory inv) {
            int circuitBreaker = 0;
            while (inv.slotsHolder.childCount > 0) {
                DestroyImmediate(inv.slotsHolder.GetChild(0).gameObject);
                circuitBreaker++;
                if (circuitBreaker > 100) {
                    Debug.LogError("using circuit breaker");
                    return;
                }
            }
        }

        public static void SetAnchors(RectTransform rectTransform) {
            if (rectTransform == null) return;
            RectTransform parent = rectTransform.parent as RectTransform;
            if (parent == null) return;
            Rect rect = parent.rect;
            
            rectTransform.anchorMin = new Vector2(
                rectTransform.anchorMin.x + rectTransform.offsetMin.x / rect.width,
                rectTransform.anchorMin.y + rectTransform.offsetMin.y / rect.height);
            rectTransform.anchorMax = new Vector2(
                rectTransform.anchorMax.x + rectTransform.offsetMax.x / rect.width,
                rectTransform.anchorMax.y + rectTransform.offsetMax.y / rect.height);

            rectTransform.offsetMin = rectTransform.offsetMax = new Vector2(0, 0);
        }

        public static void SnapItems() {
            foreach (Item item in FindObjectsOfType<Item>()) {
                if (item.currentSlot == null) continue;
                if (item == Item.draggedItem) continue;
                ItemType itemType = item.GetItemType();
                if (itemType == null) continue;

                item.graphic.raycastTarget = (Item.draggedItem == null);
                item.transform.position = item.currentSlot.transform.position;
                item.transform.SetParent(item.currentSlot.inventory.itemsHolder);
                if (item.amount > itemType.maximumStack) item.amount = itemType.maximumStack;
                if (itemType.maximumStack < 2) item.amountText.gameObject.SetActive(false);
                else {
                    item.amountText.gameObject.SetActive(true);
                    item.amountText.text = item.amount.ToString();
                }
            }
        }
        #endif
    }
}