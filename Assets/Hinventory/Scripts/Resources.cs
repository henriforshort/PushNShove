using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hiloqo.Hinventory {
    // [CreateAssetMenu(fileName = "New", menuName = "New", order = 1)]
    public class Resources : ScriptableObject {
        public List<ItemType> itemTypes;
    }
    
    [Serializable]
    public class ItemType {
        public Item.Type type;
        public int maximumStack;
    }
}
