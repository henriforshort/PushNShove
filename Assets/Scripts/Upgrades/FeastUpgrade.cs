using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeastUpgrade : Upgrade {
    public float weightGain;
    
    public override void Apply() {
        B.m.heroes.Random().unit.weight += weightGain;
    }
}