﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUpgrade : Upgrade {
    public float damageGain;
    
    public override void Apply() {
        B.m.heroes.Random().damage += damageGain;
    }
}