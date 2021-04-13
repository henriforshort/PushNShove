using UnityEngine;

public class UnitUltArcher : UnitUlt {
    [Header("Balancing")]
    public GameObject arrows;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_ARCHER_AIM);
        unit.lockAnim = true;
        unit.lockPosition = true;
        Game.m.PlaySound(MedievalCombat.BOW_DRAW_5);
        this.Wait(1, Shoot);
    }

    public override void EndUlt() {
        unit.lockPosition = false;
        unit.lockAnim = false;
    }

    public void Shoot() {
        unit.lockAnim = false;
        unit.SetAnim(Unit.Anim.ULT_ARCHER_SHOOT);
        unit.lockAnim = true;
        this.For(7, () => Game.m.PlaySound(MedievalCombat.ARROW_FLY_1), 0.1f);
        Instantiate(arrows, 
            transform.position + new Vector3(.6f, 1, 0), 
            transform.rotation, 
            Battle.m.transform);
    }
}