using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level<T> : MonoBehaviour 
    where T : Component {//Level manager, handles a single level, like battle or startMenu
                                //Should contain only Balancing and Scene References relative to this level.
                                //Should contain only State info to be deleted at the end of the level.
    public static T m;

    public void Awake() {
        if (m == null) m = GetComponent<T>();
        if (m != GetComponent<T>()) Destroy(gameObject);
    }
}
