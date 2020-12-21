using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightPrompt : MonoBehaviour {
    [Header("Balancing")]
    public float battleAppearDelay;
    public float battleDuration;
    public float fightAppearDelay;
    public float fightDuration;
    public float fadeDuration;
    
    [Header("State")]
    public float battleExpirationDate;
    public float fightExpirationDate;
    public float fightAppearDate;
    public float battleAppearDate;
    
    [Header("References")]
    public TMP_Text fightText;
    public TMP_Text battleText;
    public List<Image> battleImages;
    public UITransition bg;
    
    public void Start() {
        battleText.text = "Battle " + R.m.save.battle.AtLeast(1);
        battleExpirationDate = Time.time + battleDuration;
        fightExpirationDate = Time.time + fightDuration;
        fightAppearDate = Time.time + fightAppearDelay;
        battleAppearDate = Time.time + battleAppearDelay;
        fightText.gameObject.SetActive(false);
    }

    public void Update() {
        if (Time.time > fightAppearDate) fightText.gameObject.SetActive(true);
        if (Time.time > battleAppearDate) battleText.gameObject.SetActive(true);
        
        if (Time.time > battleExpirationDate) {
            float alpha = Time.time.Prel(battleExpirationDate + fadeDuration, battleExpirationDate);
            battleText.alpha = alpha;
            battleImages.ForEach(i => i.SetAlpha(alpha));
        }
        if (Time.time > fightExpirationDate) {
            fightText.alpha = Time.time.Prel(fightExpirationDate + fadeDuration/2, fightExpirationDate);
            if (bg.currentAnim != UITransition.Anim.FADE_OUT) bg.FadeOut();
        }
        
        if (Time.time > fightExpirationDate + fadeDuration) StartGame();
    }

    public void StartGame() {
        B.m.gameState = B.State.PLAYING;
        Destroy(gameObject);
    }
}
