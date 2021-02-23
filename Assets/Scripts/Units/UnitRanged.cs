using UnityEngine;

public class UnitRanged : UnitBehavior {
    [Header("Balancing")]
    public float range;
    public MedievalCombat aimSound;
    public MedievalCombat shootSound;
    public float aimDuration;
    public float reloadDuration;

    [Header("State")]
    public AttackStatus attackStatus;
    public float timeTillReloaded;

    [Header("References")]
    public Arrow arrowPrefab;
    
    public enum AttackStatus { READY, ATTACKING, RECOVERING }

    public void Awake() {
        attackStatus = AttackStatus.READY;
        unit.OnTakeCollisionDamage.AddListener(ResetReload);
        transform.position = new Vector3(this.Random(Game.m.spawnPosXRangeForRanged.x, 
                Game.m.spawnPosXRangeForRanged.y).ReverseIf(unit.isHero), -3, 0);
    }

    public void Update() {
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
        if (unit.enemies.Exists(e => DistanceToMe(e) <= range)) unit.currentSpeed = 0;
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

    public void UpdateCombat() {
        if (attackStatus != AttackStatus.READY) return;
        if (unit.status != Unit.Status.ALIVE) return;
        if (Battle.m.gameState == Battle.State.PAUSE) return;
        if (unit.currentSpeed.isClearlyNot(0)) return;
        if (!unit.enemies.Exists(e => DistanceToMe(e) <= range)) return;
        
        PrepareAttack();
    }

    public void PrepareAttack() {
        Game.m.PlaySound(aimSound, .25f);
        attackStatus = AttackStatus.ATTACKING;
        unit.SetAnim(Unit.Anim.PREPARE);
        this.Wait(aimDuration, Attack);
    }

    public void Attack() {
        if (unit.status != Unit.Status.ALIVE) return;
        if (attackStatus != AttackStatus.ATTACKING) return;
        
        Game.m.PlaySound(shootSound);
        attackStatus = AttackStatus.RECOVERING;
        unit.SetAnim(Unit.Anim.HIT);
        Arrow arrow = Instantiate(arrowPrefab, 
            transform.position + new Vector3(13/36f, 21/36f), 
            Quaternion.identity, 
            transform);
        arrow.owner = unit;
        ResetReload();
    }

    public float DistanceToMe(Unit other) => (this.GetX() - other.GetX()).Abs();
}