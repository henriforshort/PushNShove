using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherUltArrow : MonoBehaviour {
    [Header("Balancing")]
    public float horizontalSpeed;
    public float startVerticalSpeed;
    public float gravity;
    public float horizDuration;

    [Header("State")]
    public Vector3 startPosition;
    public float startDate;
    public bool isMoving;
    public float peakDate;
    public float duration;
    
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Sprite diagUpSprite;
    public Sprite horizontalSprite;
    public Sprite diagDownSprite;
    public Unit archer;

    public static List<Unit> potentialTargets;

    public void Start() {
        spriteRenderer.sprite = diagUpSprite;
        startDate = Time.time + this.Random(-.1f, .1f);
        startPosition = transform.position;
        isMoving = true;
        peakDate = startVerticalSpeed / gravity;
        potentialTargets = Unit.monsterUnits.Clone();
        //with current parameters the arrows take about 1.26 secs to fall
        //the "clean" way involved solving degree 2 equations and I'm too lazy for that
        horizontalSpeed += (this.GetX() - Unit.monsterUnits.Average(m => m.GetX())).Abs()/1.26f;
    }

    public void Update() {
        if (!isMoving) return;

        duration = Time.time - startDate;
        UpdatePosition();
        UpdateVisuals();
        CheckForTargets();
    }

    public void UpdatePosition() {
        this.SetX(startPosition.x + horizontalSpeed * duration);
        this.SetY(startPosition.y + startVerticalSpeed * duration - gravity * duration * duration * .5f);
        if (this.GetY() <= -2.6944444f) {
            Game.m.PlaySound(MedievalCombat.STAB_7);
            isMoving = false;
            this.SetY(-2.6944444f);
            this.Wait(this.Random(2f, 3f), () => Destroy(gameObject));
        }
    }

    public void UpdateVisuals() {
        if (duration < peakDate - horizDuration) spriteRenderer.sprite = diagUpSprite;
        else if (duration > peakDate + horizDuration) spriteRenderer.sprite = diagDownSprite;
        else spriteRenderer.sprite = horizontalSprite;
    }

    public void CheckForTargets() {
        Unit target = potentialTargets.FirstOrDefault(m => 
            (this.GetX() - m.GetX()).Abs() < .2f && (this.GetY() - m.GetY()).Abs() < m.size);
        if (target == null) return;
        
        target.GetBumpedBy(0, archer.data.damage * 1.5f, archer.data.strength * .3f);
        Destroy(gameObject);
        potentialTargets.Remove(target);
        Game.m.PlaySound(MedievalCombat.STAB_7);
    }
}