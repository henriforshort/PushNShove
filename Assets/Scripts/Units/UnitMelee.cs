using UnityEngine;

public class UnitMelee : UnitBehavior {
    public void Awake() {
        unit.currentSpeed = Game.m.unitMaxSpeed;
    }

    public void Update() {
        UpdateSpeed();
    }

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;
        
        float newSpeed = unit.currentSpeed.LerpTo(Game.m.unitMaxSpeed, Game.m.bumpRecoverySpeed);
        if (unit.currentSpeed.isAboutOrLowerThan(0) && newSpeed.isClearlyPositive()) StartWalking();
        unit.currentSpeed = newSpeed.AtMost(Game.m.unitMaxSpeed);
    }

    public bool CanUpdateSpeed() {
        if (unit.lockPosition) return false;
        if (unit.isOnFreezeFrame) return false;
        if (Battle.m.gameState == Battle.State.PAUSE) return false;
        if (unit.status == Unit.Status.FALLING) return false;
        if (unit.currentSpeed.isClearlyNegative()) return false;

        return true;
    }

    public void StartWalking() {
        unit.SetAnim(Unit.Anim.WALK);
        this.SetZ(0f);
        if (unit.status == Unit.Status.DYING) unit.DieDuringBattle();
        else Game.m.SpawnFX(Run.m.bumpDustFxPrefab,
            new Vector3(this.GetX() - 0.5f.ReverseIf(unit.isMonster), -2, -2),
            unit.isHero, 0.5f);
    }
}