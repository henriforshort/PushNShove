using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour {
    public Item item;
    public List<StatModifier> currentEffects = new List<StatModifier>();
    public virtual void Apply() { }

    public void Cancel() {
        currentEffects.ForEach(e => e.Terminate());
        currentEffects.Clear();
    }
}
