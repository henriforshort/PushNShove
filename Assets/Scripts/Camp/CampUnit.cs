using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUnit : MonoBehaviour {
    public Status status;
    public GameObject idle;
    public GameObject sleeping;
    public GameObject ready;
    
    public enum Status { IDLE, SLEEPING, READY }

    public void SetStatus(Status newStatus) {
        List<GameObject> statuses = new List<GameObject>{ idle, sleeping, ready };
        GameObject newGo;
        if (newStatus == Status.IDLE) newGo = idle;
        else if (newStatus == Status.SLEEPING) newGo = sleeping;
        else newGo = ready;

        newGo.SetActive(true);
        statuses.Except(newGo).ForEach(s => s.SetActive(false));
    }
}
