using System.Linq;
using UnityEngine;

public class UnitUltArcher : UnitUlt {
    [Header("Balancing")]
    public Projectile bigArrowPrefab;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_ARCHER_AIM);
        unit.lockAnim = true;
        unit.lockPosition = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.BOW_DRAW_5);
        this.Wait(Game.m.ultAnimDuration, Shoot);
    }

    public override void EndUlt() {
        unit.lockPosition = false;
        unit.lockAnim = false;
        unit.isInvincible = false;
    }

    public void Shoot() {
        unit.lockAnim = false;
        unit.SetAnim(Unit.Anim.HIT);
        unit.lockAnim = true;

        Game.m.PlaySound(MedievalCombat.ARROW_FLY_1);
        Projectile projectile = Instantiate(bigArrowPrefab, 
            transform.position + new Vector3(13/36f, 21/36f), 
            Quaternion.identity, 
            Battle.m.transform);
        projectile.Init(unit.data.critChance, unit.data.damage * 5, unit.data.strength * 2);
    }
}