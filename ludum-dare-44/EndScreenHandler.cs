using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreenHandler : MonoBehaviour {

    public Image blackout;
    public TextMeshProUGUI endGameText;
    public TextMeshProUGUI endGameReason;

    public TitleScreen title;
    public NPCManager npcManager;

    public AudioSource audioSource;


    public void DoEndScreen (string mainText, string reasonText) {

        endGameText.text = mainText;
        endGameReason.text = reasonText;
        endGameText.gameObject.SetActive(true);
        endGameReason.gameObject.SetActive(true);

        StartCoroutine("fadeOut");

    }

    IEnumerator fadeOut () {

        float a = 0;

        while (a < 1) {

            a += Time.deltaTime;
            blackout.color = new Color(0, 0, 0, a);
            endGameText.color = new Color(endGameText.color.r, endGameText.color.g, endGameText.color.b, a);
            endGameReason.color = new Color(endGameReason.color.r, endGameReason.color.g, endGameReason.color.b, a);
            audioSource.volume = 1f - a;
            yield return null;

        }

        yield return new WaitForSeconds(3f);

        npcManager.Reset();
        title.Init();

        while (a > 0) {

            a -= Time.deltaTime;
            blackout.color = new Color(0, 0, 0, a);
            endGameText.color = new Color(endGameText.color.r, endGameText.color.g, endGameText.color.b, a);
            endGameReason.color = new Color(endGameReason.color.r, endGameReason.color.g, endGameReason.color.b, a);
            audioSource.volume = 1f - a;
            yield return null;

        }
        endGameText.gameObject.SetActive(false);
        endGameReason.gameObject.SetActive(false);
    }

}
