using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hanimator : MonoBehaviour {
    [Header("Balancing")]
    public bool autoPlay;
    public WhenAnimFinishes whenAnimFinishes;
    public SpriteRenderer spriteRenderer;
    public Image image;
    public List<Hanimation> anims;
    
    [Header("State")]
    public bool playing;
    public Hanimation currentAnim;
    public int currentFrame;
    public float startAnimDate;
    
    public enum WhenAnimFinishes { PAUSE, HIDE, DESTROY }

    public void Start() {
        if (image == null && spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) image = GetComponent<Image>();
        if (image == null && spriteRenderer == null) 
            Debug.LogError("Hanimator does not have a target graphic", gameObject);
        
        if (autoPlay) {
            playing = true;
            Play(anims[0]);
        }
    }

    public void Update() {
        if (!playing) return;
        if ((Time.time - startAnimDate) / (currentAnim.frameDuration / 1000) < currentFrame + 1) return;
        
        currentFrame += 1;
        if (currentFrame >= currentAnim.sprites.Count) {
            if (currentAnim.loop) {
                startAnimDate = Time.time + (currentAnim.delay/1000);
                currentFrame = 0;
            } else {
                if (whenAnimFinishes == WhenAnimFinishes.HIDE) SetVisible(false);
                if (whenAnimFinishes == WhenAnimFinishes.DESTROY) Destroy(gameObject);
                playing = false;
                return;
            }
        }
        SetSprite(currentAnim.sprites[currentFrame]);
    }

    public void Play(string s) {
        Hanimation anim = anims.FirstOrDefault(a => a.name == s);
        if (anim == null) Debug.LogError("Anim not found: " + s, gameObject);
        else Play(anim);
    }

    public void Play(Hanimation a) {
        if (!anims.Contains(a)) {
            Debug.Log("anim doesnt exist");
            return;
        }

        SetVisible(true);
        playing = true;
        currentAnim = a;
        startAnimDate = Time.time;
        currentFrame = 0;
        if (a.randomStart) {
            currentFrame = this.Random(a.sprites.Count);
            startAnimDate -= a.frameDuration * currentFrame;
        }
        SetSprite(a.sprites[0]);
    }

    public void SetSprite(Sprite s) {
        if (image != null) image.sprite = s;
        else spriteRenderer.sprite = s;
    }

    public void SetVisible(bool b) {
        if (image != null) image.enabled = b;
        else spriteRenderer.enabled = b;
    }
}

[Serializable]
public class Hanimation {
    public string name;
    public float frameDuration;
    public List<Sprite> sprites;
    public bool loop;
    public float delay;
    public bool randomStart;
}
    
