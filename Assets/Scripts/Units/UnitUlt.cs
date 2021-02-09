using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitUlt : MonoBehaviour {
    [HideInInspector] public Unit unit;
    
    
    public virtual void Ult() { }
    public virtual void EndUlt() { }
}