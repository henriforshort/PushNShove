using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {
    public Hero hero;
    public Rarity rarity;
    public Item prefab;
    
    public enum Rarity { COMMON, RARE, LEGGY }

    public void ApplyEffect() {
        Debug.Log("apply effect of " + name);
    }
}