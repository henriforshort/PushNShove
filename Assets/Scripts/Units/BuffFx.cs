using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFx : MonoBehaviour {
    public Unit source;
    public Unit target;
    public float durationLeft;
    public StatModifier buff;
    
    public void Init(Unit source, Unit target, float duration, StatModifier buff) {
        this.source = source;
        this.target = target;
        this.buff = buff;
        
        durationLeft = duration;
        target.onDeath.AddListener(() => buff?.Terminate());
        source.onDeath.AddListener(() => buff?.Terminate());
    }

    public void Update() {
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        
        durationLeft -= Time.deltaTime;
        if (durationLeft < 0) EndBuff();
    }

    public void EndBuff() {
        buff.Terminate();
        Destroy(gameObject);
    }
}
