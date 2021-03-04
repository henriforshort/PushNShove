using UnityEngine;
using UnityEngine.Events;

public class UnitRanged : UnitBehavior {
    [Header("Balancing")]
    public float distanceFromScreenEdge;
    public float aimDuration;
    public float reloadDuration;

    [Header("State")]
    public AttackStatus attackStatus;
    public float timeTillReloaded;
    
    public enum AttackStatus { READY, ATTACKING, RECOVERING }

    public void Awake() {
        attackStatus = AttackStatus.READY;
        unit.OnTakeCollisionDamage.AddListener(ResetReload);
        transform.position = new Vector3(this.Random(Game.m.spawnPosXRangeForRanged.x, 
                Game.m.spawnPosXRangeForRanged.y).ReverseIf(unit.isHero), -3, 0);
    }

    public void Update() {
        if (unit.status == Unit.Status.DEAD) return;

        UpdateSpeed();
        UpdateReload();
        UpdateCombat();
    }
    
    
    // ====================
    // MOVEMENT
    // ====================

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;

        //Move forward if no enemy is in range, stop otherwise
        unit.speedLastFrame = unit.currentSpeed;
        if (IsOnScreen()) unit.currentSpeed = 0;
        else {
            unit.currentSpeed = unit.currentSpeed.LerpTo(Game.m.unitMaxSpeed, Game.m.bumpRecoverySpeed);
            attackStatus = AttackStatus.READY;
        }
        if (unit.currentSpeed.isClearlyPositive()) unit.SetAnim(Unit.Anim.WALK);
    }

    public bool CanUpdateSpeed() {
        if (unit.lockPosition) return false;
        if (unit.isOnFreezeFrame) return false;
        if (Battle.m.gameState == Battle.State.PAUSE) return false;
        if (unit.status == Unit.Status.FALLING) return false;
        if (unit.currentSpeed.isClearlyNegative()) return false;
        if (attackStatus != AttackStatus.READY) return false;

        return true;
    }

    public bool IsOnScreen() => 
        (this.GetX() - Battle.m.cameraManager.cameraFocus.GetX()).Abs() < 7f - distanceFromScreenEdge;

    
    // ====================
    // RELOADING
    // ====================

    public void ResetReload() {
        timeTillReloaded = reloadDuration;
        attackStatus = AttackStatus.RECOVERING;
    }

    public void UpdateReload() {
        if (attackStatus != AttackStatus.RECOVERING) return;
        if (Battle.m.gameState == Battle.State.PAUSE) return;
        if (unit.hero.ultStatus == UnitHero.UltStatus.ACTIVATED) return;

        timeTillReloaded -= Time.deltaTime;
        if (timeTillReloaded <= 0) attackStatus = AttackStatus.READY;
    }
    
    
    // ====================
    // COMBAT
    // ====================

    [HideInInspector] public UnityEvent OnAim;
    [HideInInspector] public UnityEvent OnAttack;

    public void UpdateCombat() {
        if (attackStatus != AttackStatus.READY) return;
        if (unit.status != Unit.Status.ALIVE) return;
        if (Battle.m.gameState == Battle.State.PAUSE) return;
        if (unit.currentSpeed.isClearlyNot(0)) return;
        if (!IsOnScreen()) return;
        
        PrepareAttack();
    }

    public void PrepareAttack() {
        attackStatus = AttackStatus.ATTACKING;
        if (aimDuration.isClearlyPositive()) unit.SetAnim(Unit.Anim.PREPARE);
        this.Wait(aimDuration, Attack);
    }

    public void Attack() {
        if (unit.status != Unit.Status.ALIVE) return;
        if (attackStatus != AttackStatus.ATTACKING) return;

        OnAttack.Invoke();
        unit.SetAnim(Unit.Anim.HIT);
        attackStatus = AttackStatus.RECOVERING;
        ResetReload();
    }
}