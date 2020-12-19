using UnityEngine;

public class Hero : MonoBehaviour {
    [Header("Balancing")]
    public float ultCooldown;
    public float ultDuration;
    
    [Header("State")]
    public float ultCooldownLeft;
    public UltStatus ultStatus;
    
    [Header("References")]
    public Unit unit;
    public Sprite image;
    public HeroIcon icon;
    
    public enum UltStatus { AVAILABLE, RELOADING, ACTIVATED }

    public void Start() {
        ultStatus = UltStatus.RELOADING;
    }

    public void Update() {
        UpdateUlt();
    }

    public void InitIcon(HeroIcon hi) {
        icon = hi;
        icon.hero = this;
        icon.icon.sprite = image;
    }

    public void UpdateUlt() {
        if (B.m.gameState != B.State.PLAYING) return;
        if (unit.status != Unit.Status.ALIVE) return;
        if (ultStatus != UltStatus.RELOADING) return;
        
        ultCooldownLeft -= Time.deltaTime;
        if (ultCooldownLeft < 0) ReadyUlt();
    }

    public void ReadyUlt() {
        ultCooldownLeft = 0;
        ultStatus = UltStatus.AVAILABLE;
        icon.ReadyUlt();
    }

    public bool CanUlt() {
        if (B.m.gameState != B.State.PLAYING) return false;
        if (ultStatus != UltStatus.AVAILABLE) return false;

        return true;
    }

    public void Ult() {
        ultStatus = UltStatus.ACTIVATED;
        unit.Ult();
        this.Wait(ultDuration, EndUlt);
        unit.lockZOrder = true;
    }

    public void EndUlt() {
        ultStatus = UltStatus.RELOADING;
        ultCooldownLeft = ultCooldown;
        unit.EndUlt();
        icon.StartUltReload();
        unit.lockZOrder = false;
    }
}
