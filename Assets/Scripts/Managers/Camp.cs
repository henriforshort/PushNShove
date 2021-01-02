using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : Level<Camp> {
    public void StartBattle() => Game.m.LoadScene(Game.SceneName.Battle);
}
