using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NewInfo : MonoBehaviour {

    public TextMeshProUGUI textMesh;
    bool isActive;

    public void Activate() {

        if (!isActive)
            StartCoroutine("fadeInAndOut");

    }

    IEnumerator fadeInAndOut () {

        float a = 0;
        isActive = true;

        while (a < 1) {

            a += Time.deltaTime;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, a);
            yield return null;

        }

        yield return new WaitForSeconds(1f);

        while (a > 0) {

            a -= Time.deltaTime;
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, a);
            yield return null;

        }

        isActive = false;

    }

}
