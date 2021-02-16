using UnityEngine;

public class UnitRanged : UnitBehavior {
    [Header("Balancing")]
    public float range;

    [Header("State")]
    public AttackStatus attackStatus;
    
    public enum AttackStatus { NOT_PREPARED, PREPARING, ATTACKING, RECOVERING }

    public void Start() {
        attackStatus = AttackStatus.NOT_PREPARED;
    }

    public void Update() {
        UpdateVisuals();
        UpdateSpeed();
        UpdateCombat();
    }

    public void UpdateVisuals() {
        
        if (unit.speedLastFrame.isAbout(0) && unit.currentSpeed.isClearlyNot(0)) {
            unit.SetAnim(Unit.Anim.WALK);
        }
        if (unit.anim != Unit.Anim.HIT && unit.anim != Unit.Anim.PREPARE) attackStatus = AttackStatus.NOT_PREPARED;
    }

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;

        //Move forward if no enemy is in range, stop otherwise
        unit.speedLastFrame = unit.currentSpeed;
        if (unit.enemies.Exists(e => DistanceToMe(e) <= range)) unit.currentSpeed = 0;
        else {
            unit.currentSpeed = unit.currentSpeed.LerpTo(Game.m.unitMaxSpeed, Game.m.bumpRecoverySpeed);
            attackStatus = AttackStatus.NOT_PREPARED;
        }
    }

    public bool CanUpdateSpeed() {
        if (unit.lockPosition) return false;
        if (unit.isOnFreezeFrame) return false;
        if (Battle.m.gameState == Battle.State.PAUSE) return false;
        if (unit.status == Unit.Status.FALLING) return false;
        if (unit.currentSpeed.isClearlyNegative()) return false;

        return true;
    }

    public void UpdateCombat() {
        if (!unit.enemies.Exists(e => DistanceToMe(e) <= range)) return;
        if (unit.currentSpeed.isClearlyNot(0)) return;
        if (attackStatus != AttackStatus.NOT_PREPARED) return;
        
        PrepareAttack();
    }

    public void PrepareAttack() {
        Debug.Log("prepare attack");
        attackStatus = AttackStatus.PREPARING;
        unit.SetAnim(Unit.Anim.PREPARE);
        this.Wait(1f, Attack);
    }

    public void Attack() {
        Debug.Log("hit");
        unit.SetAnim(Unit.Anim.HIT);
        this.Wait(0.5f, () => {
            unit?.enemies?.WithLowest(DistanceToMe)?.GetBumpedBy(unit);
            attackStatus = AttackStatus.NOT_PREPARED;
        });
    }
    
    public float DistanceToMe(Unit other) => (this.GetX() - other.GetX()).Abs();
}