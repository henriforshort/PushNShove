using System.Collections.Generic;
using UnityEngine;

public class UnitRangedBuff : MonoBehaviour {
    [Header("Balancing")]
    public float buffDuration;
    public float attackAnimDuration;
    
    // [Header("Status")]

    [Header("References")]
    public GameObject buffFx;
    public UnitRanged unitRanged;

    public void Awake() {
        unitRanged.OnAttack.AddListener(Attack);
    }

    public void Attack() {
        List<StatModifier> currentModifiers = null;
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        this.Wait(attackAnimDuration, () => currentModifiers = ApplyBuff());
        this.Wait(buffDuration, () => currentModifiers?.ForEach(m => m.Terminate()));
    }

    public List<StatModifier> ApplyBuff() {
        List<StatModifier> modifiers = new List<StatModifier>();
        unitRanged.unit.allies
            .Except(unitRanged.unit)
            .ForEach(target => {
                Game.m.SpawnFX(buffFx, new Vector3(target.GetX(), target.GetY() + .7f, 8f), 
                    false, buffDuration, target.transform);
                modifiers.Add(target.data.strength.AddModifier(.5f));
        });
        return modifiers;
    }
}