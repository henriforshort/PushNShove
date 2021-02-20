using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Sprite diagUpSprite;
    public Sprite horizontalSprite;
    public Sprite diagDownSprite;

    public void Start() {
        spriteRenderer.sprite = diagUpSprite;
        startDate = Time.deltaTime;
        startPosition = transform.position;
        isMoving = true;
        peakDate = startVerticalSpeed / gravity;
    }

    public void Update() {
        if (!isMoving) return;

        float duration = Time.time - startDate;
        this.SetX(startPosition.x + horizontalSpeed * duration);
        this.SetY(startPosition.y + startVerticalSpeed * duration - gravity * duration * duration * .5f);
        if (this.GetY() <= -2.6944444f) isMoving = false;

        if (duration < peakDate - horizDuration) spriteRenderer.sprite = diagUpSprite;
        else if (duration > peakDate + horizDuration) spriteRenderer.sprite = diagDownSprite;
        else spriteRenderer.sprite = horizontalSprite;
    }
}