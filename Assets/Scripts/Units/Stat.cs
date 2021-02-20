using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stat {
    public float startValue;
    [SerializeField] public float value;
    [SerializeField] public List<StatModifier> modifiers;

    public static implicit operator float(Stat s) => s.value;

    public Stat() {
        modifiers = new List<StatModifier>();
    }

    public void Init(float val) {
        startValue = val;
        modifiers.Clear();
        UpdateValue();
    }

    public StatModifier AddModifier(float val = 0, StatModifier.Type type = StatModifier.Type.ADD, 
        StatModifier.Scope scope = StatModifier.Scope.BATTLE, int priority = 0) {
        StatModifier modifier = new StatModifier(val, type, scope, priority, this);
        
        modifiers.Add(modifier);
        UpdateValue();
        return modifier;
    }

    public void RemoveModifier(Guid guid) {
        modifiers.RemoveAll(m => m.guid == guid);
        UpdateValue();
    }

    public void PurgeModifiers(StatModifier.Scope scope) {
        modifiers.RemoveAll(mod => mod.scope == scope);
        UpdateValue();
    }

    private void UpdateValue() {
        float result = startValue;
        List<StatModifier> modifiersClone = modifiers.Clone();
        int circuitBreaker = 0;
        while (modifiersClone.Count > 0) {
            modifiersClone//Get list of stat modifiers with lowest priority
                .Where(m => m.priority == modifiersClone.Select(n => n.priority).Min())
                .ToList()
                .ForEach(m => {//Apply each one of them, and remove them from original list
                    if (m.type == StatModifier.Type.ADD) result += m.value;
                    else if (m.type == StatModifier.Type.MULTIPLY) result *= m.value;
                    else if (m.type == StatModifier.Type.SET) result = m.value;
                    modifiersClone.Remove(m);
                });
            circuitBreaker++;
            if (circuitBreaker > 100) {
                Debug.LogError("using circuit breaker");
                break;
            }
        }
        value = result;
    }

    private float ApplyModifier(float stat, StatModifier modifier) {
        if (modifier.type == StatModifier.Type.ADD) return stat + modifier.value;
        else if (modifier.type == StatModifier.Type.MULTIPLY) return stat * modifier.value;
        else if (modifier.type == StatModifier.Type.SET) return modifier.value;
        return 0;
    }
}

[Serializable]
public class StatModifier {
    public Guid guid;
    public Type type;
    public float value;
    public Scope scope;
    public int priority; //Lowest priority is applied first
    [NonSerialized] private Stat stat;
    
    public enum Type { ADD, MULTIPLY, SET }
    public enum Scope { BATTLE, RUN, GAME }

    public StatModifier(float value, Type type, Scope scope, int priority, Stat stat) {
        guid = Guid.NewGuid();
        this.type = type;
        this.value = value;
        this.scope = scope;
        this.priority = priority;
        this.stat = stat;
    }

    public void Terminate() => stat.RemoveModifier(guid);
}
