using UnityEngine;

public class UnitMelee : UnitBehavior {
    [Header("State")]
    public AttackStatus attackStatus;
    
    [HideInInspector] public float attackAnimDuration = 0.25f;
    [HideInInspector] public float attackSpeed = 0.5f;
    public static float lastSparkFxDate;

    public enum AttackStatus { NOT_PREPARED, PREPARING, ATTACKING, RECOVERING }
    
    
    // ====================
    // BASICS
    // ====================
    
    
    public void Awake() {
        unit.currentSpeed = Game.m.unitMaxSpeed;
    }

    public void Update() {
        UpdateSpeed();
        UpdateCombat();
        UpdateVisuals();
    }
    
    
    // ====================
    // UPDATES
    // ====================

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;
        
        float newSpeed = unit.currentSpeed.LerpTo(Game.m.unitMaxSpeed, Game.m.bumpRecoverySpeed);
        if (unit.currentSpeed.isAboutOrLowerThan(0) && newSpeed.isClearlyPositive()) StartWalking();
        unit.currentSpeed = newSpeed.AtMost(Game.m.unitMaxSpeed);
    }

    public void UpdateCombat() { //Called by both sides
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        
        if (CanPrepareAttack()) PrepareAttack();
        Unit collidingEnemy = CollidingEnemy();
        if (CanBumpWithoutAttack(collidingEnemy)) BumpWithoutAttack(collidingEnemy);
    }
    
    public void UpdateVisuals() {
        if (unit.lockZOrder) return;
        if (attackStatus == AttackStatus.ATTACKING) return;
        if (this.GetZ().isClearlyNot(-1)) return;
        
        this.SetZ(-0.5f);
    }
    
    
    // ====================
    // SPEED UPDATE
    // ====================

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
    
    
    // ====================
    // BUMP WITHOUT ATTACK
    // ====================

    public bool CanBumpWithoutAttack(Unit other) {
        if (!unit.isWalking) return false;
        if (attackStatus == AttackStatus.PREPARING) return false;
        if (other == null) return false;
        if (!other.isWalking) return false;
        if (other.melee?.attackStatus == AttackStatus.PREPARING) return false;
        
        return true;
    }

    public void BumpWithoutAttack(Unit other) {
        unit.SetAnim(Unit.Anim.DEFEND);
        DefendFrom(other);
        other.SetAnim(Unit.Anim.DEFEND);
        other.melee.DefendFrom(unit);
        Game.m.PlaySound(MedievalCombat.METAL_WEAPON_HIT_METAL_1);
    }
    
    
    // ====================
    // ATTACK
    // ====================

    public bool CanPrepareAttack() {
        if (!unit.isWalking) return false;
        if (attackStatus != AttackStatus.NOT_PREPARED) return false;
        if (NearbyEnemy() == null) return false;
        
        return true;
    }

    public void PrepareAttack() {//Called by both sides
        unit.SetAnim(Unit.Anim.WINDUP);
        attackStatus = AttackStatus.PREPARING;
        this.SetZ(-1);
        unit.FreezeFrame();
        this.Wait(0.1f, TryAttack);
    }

    public void TryAttack() {//Called by both sides
        if (attackStatus != AttackStatus.PREPARING) return;
        
        Unit target = NearbyEnemy();
        if (target == null) Attack();
        else ResolveCombat(unit, target);
    }

    public void Attack() {
        Game.m.PlaySound(MedievalCombat.WHOOSH_1);
        unit.SetAnim(Unit.Anim.HIT);
        attackStatus = AttackStatus.ATTACKING;
        this.Wait(attackAnimDuration, RecoverFromAttack);
    }

    public void ResolveCombat(Unit unit1, Unit unit2) { //Called by attacking side only
        Unit winner = GetAttackWinner(unit1, unit2);
        Unit loser = (winner == unit1 ? unit2 : unit1); 
        
        winner.melee?.Attack();
        loser.melee?.Attack();
        
        loser.GetBumpedBy(winner);
        winner.melee?.DefendFrom(loser);

        if (winner.shakeOnHit) Battle.m.cameraManager.Shake(0.2f);
        this.Wait(0.1f, () => {
            Game.m.PlaySound(winner.attackSound);
            Game.m.PlaySound(winner.attackSoundAnimal, .5f, -1, unit.pitch);
            if (.5f.Chance()) {
                Game.m.PlaySound(
                    this.Random(winner, loser).deathSoundHuman, .5f, -1, unit.pitch);
                Game.m.PlaySound(
                    this.Random(winner, loser).deathSoundAnimal, .5f, -1, unit.pitch);
            }
            if (winner.size > 1) Game.m.PlaySound(MedievalCombat.BODY_FALL);
        });
        if (Time.time - lastSparkFxDate > 0.1f) {
            lastSparkFxDate = Time.time;
            Game.m.SpawnFX(Run.m.sparkFxPrefab,
                new Vector3(this.GetX() + 2f.ReverseIf(unit.isMonster), -2, -2),
                winner.isMonster, 0.5f);
        }
    }

    public Unit GetAttackWinner(Unit unit1, Unit unit2) {
        if (unit1.isInvincible) return unit1;
        if (unit2.isInvincible) return unit2;
        
        if (unit1.melee == null) return unit2;
        if (unit2.melee == null) return unit1;
        
        if (!unit1.melee.CanAttack()) return unit2;
        if (!unit2.melee.CanAttack()) return unit1;
        
        float momentum1 = unit1.data.weight * (2 * unit1.speedPercent).Clamp01(); //max momentum if > 50% speed
        float momentum2 = unit2.data.weight * (2 * unit2.speedPercent).Clamp01();
        return Random.value < momentum1 / (momentum1 + momentum2) ? unit1 : unit2;
    }

    public void RecoverFromAttack() {
        attackStatus = AttackStatus.RECOVERING;
        this.Wait(attackSpeed, () => attackStatus = AttackStatus.NOT_PREPARED);
    }

    public void DefendFrom(Unit other) {
        unit.currentSpeed = Game.m.defendSpeed * other.data.strength * (1 - unit.data.prot);
    }

    public Unit NearbyEnemy() => unit.enemies
        .WithLowest(DistanceToMe)
        .If(e => e != null && DistanceToMe(e) < Game.m.attackDistance);

    
    public Unit CollidingEnemy() => unit.enemies
        .WithLowest(DistanceToMe)
        .If(e => e != null && DistanceToMe(e) < Game.m.collideDistance);
    
    public float DistanceToMe(Unit other) => (this.GetX() - other.GetX()).Abs();
    public bool CanAttack() => unit.isWalking && attackStatus == AttackStatus.PREPARING;
}