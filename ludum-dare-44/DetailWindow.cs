using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetailWindow : MonoBehaviour {

    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public void UpdateWindow (string _title, string _content) {

        title.text = _title;
        content.text = _content;

    }

    public void UpdateWindowTitle (string _title) {

        title.text = _title;

    }

    public void UpdateWindowContent (string _content) {

        content.text = _content;

    }

}
