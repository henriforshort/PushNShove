using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendaryEffect : ItemEffect {
    public override void Apply() {
        currentEffects.Add(
            item.hero.unit.critChance.AddModifier(0.1f, StatModifier.Type.ADD,StatModifier.Scope.RUN));
    }
}