using System.Collections.Generic;
using UnityEngine;

public class UnitUltKnight : UnitUlt {
    [Header("Prefab References")]
    public BuffFx buffFxPrefab;

    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_KNIGHT);
        Game.m.PlaySound(MedievalCombat.MAGIC_HOLY, .5f, 1);
        
        BuffFx buffFx = Instantiate(buffFxPrefab, 
            new Vector3(unit.GetX(), unit.GetY() + .4f, 0f),
            Quaternion.identity, unit.transform);
        buffFx.Init(unit, unit, unit.hero.ultDuration, 
            unit.data.block.AddModifier(1, StatModifier.Type.SET));
    }

    public override void EndUlt() {
    }
}
