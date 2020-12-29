using UnityEngine;

namespace Hiloqo.Hinventory {
    public class Inventory : MonoBehaviour {
        [Header("Assign this, then click \"Tools>Hanimator>Create Inventories\"")]
        public Vector2 size;
        public Vector2 slotSize;
        public float spacing;
        [Header("Ignore this")]
        public Transform itemsHolder;
        public Transform slotsHolder;
    }
}
