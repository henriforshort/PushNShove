using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : Level<Camp> {
    public UITransition transition;
    
    public void StartBattle() {
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
    }
}
