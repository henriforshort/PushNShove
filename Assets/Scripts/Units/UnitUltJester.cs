using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitUltJester : UnitUlt {
    [Header("References")]
    public Projectile projectilePrefab;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT);
        unit.lockAnim = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.WHOOSH_3);
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        this.Wait(unit.hero.ultAnimDuration, ApplyBuff);
    }

    public override void EndUlt() {
        unit.lockAnim = false;
        unit.isInvincible = false;
        unit.SetAnim(Unit.Anim.DEFEND);
    }

    public void ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return;
        
        //Send note
        Instantiate(projectilePrefab, 
            transform.position + new Vector3(13/36f, 21/36f), 
            Quaternion.identity, 
            Battle.m.transform)
            .Init(unit.data.critChance, unit.data.damage, unit.data.strength * 10);
    }
}