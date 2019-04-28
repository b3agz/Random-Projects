using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour {

    public Sprite[] titleImages;
    Image titleImage;

    public GameObject menu;
    public TextMeshProUGUI[] menuOptions;
    public Color unselectedColour;

    public GameObject howToPlayWindow;
    public GameObject howToP1;
    public GameObject howToP2; 
    public bool viewingHowToPlay = false;
    public Slider crowdSlider;

    public GraphicRaycaster raycaster;
    public PointerEventData pointerEventData;
    EventSystem eventSystem;
    public NPCManager npcManager;
    public PlayerController player;
    public Image fade;

    public AudioSource audioSource;
    public AudioClip titleMusic;
    public AudioClip ambientMusic;

    bool inTitleScreen = true;
    int selectedIndex = 0;

    private void Start() {

        eventSystem = GetComponent<EventSystem>();
        titleImage = GetComponent<Image>();
        Init();

    }

    public void Init () {

        gameObject.SetActive(true);
        player.isLocked = true;
        inTitleScreen = true;
        titleImage.enabled = true;
        menu.SetActive(true);
        StartCoroutine("runTitleScreen");
        audioSource.clip = titleMusic;
        audioSource.loop = true;
        audioSource.Play();

    }

    private void Update() {
        
        if (viewingHowToPlay) {

            if (Input.anyKeyDown) {

                if (howToP2.activeSelf == false) {
                    howToP1.SetActive(false);
                    howToP2.SetActive(true);
                } else {
                    viewingHowToPlay = false;
                    howToP2.SetActive(false);
                    howToPlayWindow.SetActive(false);
                }
            } else
                return;

        }

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (Input.GetKeyDown(KeyCode.Return))
            MakeSelection();

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {

            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = menuOptions.Length - 1;

        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {

            selectedIndex++;
            if (selectedIndex > menuOptions.Length - 1)
                selectedIndex = 0;

        }

        foreach (RaycastResult result in results) {
            if (result.gameObject.tag == "MainMenuItem") {

                selectedIndex = int.Parse(result.gameObject.name);

                if (Input.GetMouseButtonDown(0)) {

                    MakeSelection();

                }

            }
        }

        foreach (TextMeshProUGUI o in menuOptions)
        if (int.Parse(o.gameObject.name) == selectedIndex)
            o.color = Color.white;
        else
            o.color = unselectedColour;

    }

    void MakeSelection () {

        switch (selectedIndex) {

            case 0:
                StartCoroutine("newGame");
                break;

            case 1:
                howToPlayWindow.SetActive(true);
                howToP1.SetActive(true);
                viewingHowToPlay = true;
                break;

            case 2:
                Application.Quit();
                break;

        }

    }

    IEnumerator runTitleScreen () {

        int imageIndex = 0;
        while (inTitleScreen) {

            titleImage.sprite = titleImages[imageIndex];

            yield return new WaitForSeconds (Random.Range (2f, 6f));

            imageIndex++;
            if (imageIndex > titleImages.Length - 1)
                imageIndex = 0;

        }

    }

    IEnumerator newGame () {

        float a = 0;

        while (a < 1) {

            a += Time.deltaTime;
            fade.color = new Color(0, 0, 0, a);
            audioSource.volume = 1f - a;
            yield return null;

        }

        npcManager.Init((int)crowdSlider.value);
        
        player.isLocked = false;
        player.transform.position = new Vector3(0f, player.transform.position.y, player.transform.position.z);
        player.npcsInRange.Clear();
        inTitleScreen = false;
        titleImage.enabled = false;
        menu.SetActive(false);
        audioSource.clip = ambientMusic;
        audioSource.loop = true;
        audioSource.Play();

        while (a > 0) {

            a -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, a);
            audioSource.volume = 1f - a;
            yield return null;

        }

        gameObject.SetActive(false);

    }

}
