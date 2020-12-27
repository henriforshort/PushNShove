using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareEffect : ItemEffect {
    public override void Apply() {
        currentEffects.Add(
            item.hero.unit.prot.AddModifier(0.1f, StatModifier.Type.ADD, StatModifier.Scope.RUN));
    }
}
