using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour {
    public TMP_Text text;
    private void Start() {
        text.text = "v"+Application.version;
    }
}
