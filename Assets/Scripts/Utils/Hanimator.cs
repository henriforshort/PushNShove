using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hanimator : MonoBehaviour {
    [Header("Balancing")]
    public bool autoPlay;
    public bool isUI;
    public List<Hanimation> anims;
    
    [Header("State")]
    public bool playing;
    public Hanimation currentAnim;
    public int currentFrame;
    public float startAnimDate;
    
    [Header("Self References")]
    public SpriteRenderer spriteRenderer;
    public Image image;

    public void Start() {
        if (isUI && image == null) image = GetComponent<Image>();
        if (!isUI && spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (autoPlay) {
            playing = true;
            Play(anims[0]);
        }
    }

    public void Update() {
        if (!playing) return;
        if ((Time.time - startAnimDate) / (currentAnim.frameDuration/1000) > currentFrame) {
            currentFrame += 1;
            if (currentFrame >= currentAnim.sprites.Count) {
                if (currentAnim.loop) {
                    startAnimDate = Time.time + (currentAnim.delay/1000);
                    currentFrame = 0;
                } else {
                    playing = false;
                    return;
                }
            }
            SetSprite(currentAnim.sprites[currentFrame]);
        }
    }

    public void Play(string s) {
        Hanimation a = anims.FirstOrDefault(a => a.name == s);
    }

    public void Play(Hanimation a) {
        if (!anims.Contains(a)) {
            Debug.Log("anim doesnt exist");
            return;
        }

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
        if (isUI) image.sprite = s;
        else spriteRenderer.sprite = s;
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
    
