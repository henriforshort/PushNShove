using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheatPanel : MonoBehaviour {
    [Header("Self References")]
    public GameObject mainCheatMenu; 
    public GameObject levelCheatMenu;    

    public void OpenCheats() {
        mainCheatMenu.SetActive(false);
        levelCheatMenu.SetActive(true);
    }

    public void CloseCheats() {
        mainCheatMenu.SetActive(true);
        levelCheatMenu.SetActive(false);
    }

    public void ResetAndQuit() {
        File.Delete(Game.m.savePath); 
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
