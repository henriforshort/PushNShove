using System.Linq;
using UnityEngine;

public class UnitUltArcher : UnitUlt {
    [Header("Balancing")]
    public GameObject arrows;
    public Projectile bigArrowPrefab;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_ARCHER_AIM);
        unit.lockAnim = true;
        unit.lockPosition = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.BOW_DRAW_5);
        this.Wait(1, Shoot);
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
        // this.For(7, () => Game.m.PlaySound(MedievalCombat.ARROW_FLY_1), 0.1f);
        // Instantiate(arrows,
        //         transform.position + new Vector3(.6f, 1, 0),
        //         transform.rotation,
        //         Battle.m.transform)
        //     .transform
        //     .GetComponentsInChildren<ArcherUltArrow>()
        //     .ToList()
        //     .ForEach(a => a.archer = unit);

        Game.m.PlaySound(MedievalCombat.ARROW_FLY_1);
        Projectile projectile = Instantiate(bigArrowPrefab, 
            transform.position + new Vector3(13/36f, 21/36f), 
            Quaternion.identity, 
            Battle.m.transform);
        projectile.Init(unit.data.critChance, unit.data.damage * 5, unit.data.strength * 2);
    }
}