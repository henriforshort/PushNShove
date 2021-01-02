using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEffect : ItemEffect {
    public override void Apply() {
        currentEffects.Add(
            item.hero.unit.data.damage.AddModifier(1, StatModifier.Type.ADD, StatModifier.Scope.RUN));
    }
}
