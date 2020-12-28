using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : Level<StartMenu> {//Start Menu manager, handles the start menu
                                //Should contain only Balancing and Scene References relative to the start menu.
                                //Should contain only State info to be deleted at the end of the start menu.
    [Header("Scene References")]
    public GameObject background;
    
    public void StartGame() {
        background.SetActive(true);
        Game.m.LoadScene(Game.SceneName.Battle);
    }
}
