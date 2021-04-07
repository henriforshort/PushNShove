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
    public Hanimation currentAnim; //TODO only display name. Only store name?
    public int currentFrame;
    public float startAnimDate;
    
    public enum WhenAnimFinishes { PAUSE, LOOP, HIDE, DESTROY, PLAY_RANDOM }

    public void Awake() {
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
        if ((Time.time - startAnimDate) / (currentAnim.frameDuration/1000) < currentFrame + 1) return;

        if (currentFrame + 1 < currentAnim.sprites.Count) {
            currentFrame += 1;
            SetSprite(currentAnim.sprites[currentFrame]);
        } else {
            playing = false;
            if (currentAnim.loop) this.Wait(currentAnim.delay/1000, () => Play(currentAnim, true));
            else {
                if (whenAnimFinishes == WhenAnimFinishes.PAUSE) playing = false; //do nothing
                if (whenAnimFinishes == WhenAnimFinishes.LOOP) Play(currentAnim);
                if (whenAnimFinishes == WhenAnimFinishes.HIDE) SetVisible(false);
                if (whenAnimFinishes == WhenAnimFinishes.DESTROY) Destroy(gameObject);
                if (whenAnimFinishes == WhenAnimFinishes.PLAY_RANDOM)
                    Play(anims.WeightedRandom(anims.Select(a => a.weight).ToList()));
            }
        }
    }

    public void Play(string s) {
        Hanimation anim = anims.FirstOrDefault(a => a.name == s);
        if (anim == null) Debug.LogError("Anim not found: " + s, gameObject);
        else Play(anim);
    }

    public void Play(int i) {
        try { Play(anims[i]); }
        catch { Debug.LogError("Hanimator does not have anim with index: "+i, gameObject); }
    }

    //TODO ignore if anim is already current and hanimator is playing
    public void Play(Hanimation a, bool isLoop = false) {
        if (!anims.Contains(a)) {
            Debug.Log("anim doesnt exist");
            return;
        }

        SetVisible(true);
        playing = true;
        currentAnim = a;
        startAnimDate = Time.time;
        currentFrame = 0;
        if (!isLoop && a.randomStart) {
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
    public float delay; //TODO store in "advanced" struct
    public int weight;
    public bool loop;
    public bool randomStart;
    public List<Sprite> sprites;

    public override string ToString() {
        return name;
    }
}
    
