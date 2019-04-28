using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueHandler : MonoBehaviour {

    /*
     * Each questions array corresponds to a different NPC personality type and has the same order (order is critical).
     * 
     * 0 - Ask NPC about target, they don't know anything.
     * 1 - Ask NPC about target, they have some vague information.
     * 2 - Ask NPC about target, they have some exact information.
     * 
     * 3 - Ask NPC about themselves, they refuse to answer.
     * 4 - Ask NPC about themselves, they give vague information.
     * 5 - Ask NPC about themselves, they give specific information
     * 
     * 6 - Random chatter
     * 
     * 7 - Accuse NPC of being target, NPC is not target.
     * 8 - Accuse NPC of being target, NPC IS target.
     * 
     * 9 - Leave conversation.
     */

    public Question[] helpful; // Questions for helpful NPCs.
    public Question[] annoying; // And so on...
    public Question[] stubborn; 
    public Question[] awkward;

    public List<Question> currentDialogueOptions = new List<Question>();

    public TextMeshProUGUI speechBox;
    public GameObject DialogueOptions;
    public TextMeshProUGUI DialogueTitle;
    public Transform DialogueOptionWindow;
    List<TextMeshProUGUI> DialogueOptionTexts = new List<TextMeshProUGUI>();
    public GameObject menuItemPrefab;
    public Color unselectedItem;
    public NewInfo newInfo;

    public PlayerController player;

    public bool isTalking;
    public bool isConversing = false;
    int selectedIndex = 0;

    public GraphicRaycaster raycaster;
    public PointerEventData pointerEventData;
    EventSystem eventSystem;

    NPCController npc;
    public NPCManager npcManager;

    public AudioSource audioSource;

    private void Start() {

        eventSystem = GetComponent<EventSystem>();

    }

    private void Update() {

        if (!isTalking && !isConversing && Input.GetKeyDown(KeyCode.Escape))
            npcManager.PlayerExit();

        if (!isTalking || isConversing)
            return;

        if (Input.GetKeyDown(KeyCode.Return)) {

                 if (currentDialogueOptions[selectedIndex].simpleText == "Goodbye") // Leaving conversation.
                    StartCoroutine("Converse", 1);
                else if (currentDialogueOptions[selectedIndex].simpleText == "Accuse") {

                    if (npc.details.isTarget) // Made a correct accusation.
                        StartCoroutine("Converse", 2);
                    else
                        StartCoroutine("Converse", 3); // Made a false accusation.
                } else
                    StartCoroutine("Converse", 0); // Back to dialogue selection.

        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {

            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = currentDialogueOptions.Count - 1;

        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {

            selectedIndex++;
            if (selectedIndex > currentDialogueOptions.Count - 1)
                selectedIndex = 0;

        }

            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results) {
            if (result.gameObject.tag == "MenuItem")

                selectedIndex = int.Parse(result.gameObject.name);
                
                if (Input.GetMouseButtonDown(0)) {

                if (currentDialogueOptions[selectedIndex].simpleText == "Goodbye") // Leaving conversation.
                    StartCoroutine("Converse", 1);
                else if (currentDialogueOptions[selectedIndex].simpleText == "Accuse") {

                    if (npc.details.isTarget) // Made a correct accusation.
                        StartCoroutine("Converse", 2);
                    else
                        StartCoroutine("Converse", 3); // Made a false accusation.
                } else
                    StartCoroutine("Converse", 0); // Back to dialogue selection.

            }


        }

        foreach (TextMeshProUGUI o in DialogueOptionTexts) {

            if (int.Parse(o.gameObject.name) == selectedIndex)
                o.color = Color.white;
            else
                o.color = unselectedItem;

        }

    }

    IEnumerator Converse (int endState) {

        // endState lets us know if we're doing anything after this dialogue finishes.
        // 0 = nothing, 1 = end of conversation, 2 = target found.

        isConversing = true;
        DialogueOptions.gameObject.SetActive(false);

        string aText = "";
        string answer = ParseText(currentDialogueOptions[selectedIndex].GetAnswerText());
        audioSource.Play();

        for (int i = 0; i < answer.Length; i++) {

            aText += answer[i];
            speechBox.text = aText;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));

        }
        audioSource.Stop();

        bool waiting = true;
        bool blink = false;
        float timer = 0;
        while (waiting) {

            if (Input.anyKeyDown)
                waiting = false;
            else if (timer > 0.25f) {

                timer = 0;
                blink = !blink;

                if (blink)
                    speechBox.text = aText + "\n \\/";
                else
                    speechBox.text = aText;

            } else
                timer += Time.deltaTime;

            yield return null;

        }

        speechBox.text = "";
        isConversing = false;
        npc.details.askedQuestion();

        if (endState == 0)
            DialogueOptions.gameObject.SetActive(true);
        else if (endState == 1) {
            EndConversation();
            npcManager.NPCAsked();
        } else if (endState == 2) {
            EndConversation();
            npcManager.FoundThem();
        } else if (endState == 3) {
            EndConversation();
            npcManager.GameOver();
        }
    }

    string ParseText (string text) {

        string buffer = "";
        int charCount = 0;

        // Loop through each character in the text.
        while (charCount < text.Length) {

            // If we hit a #, there's a tag that needs replacing with a contextual word.
            if (text[charCount].ToString() == "#") {
                charCount++;
                bool endOfTag = false;
                string tag = "";
                while (!endOfTag && charCount < text.Length) {
                    // If we hit another #, we're at the end of the tag.
                    if (text[charCount].ToString() == "#") {
                        endOfTag = true;
                        buffer += SwapTag(tag);
                        charCount++;
                    } else {
                        tag += text[charCount];
                        charCount++;
                    }
                }
            }

            buffer += text[charCount];
            charCount++;

        }

        return buffer;

    }

    void buildDialogueOptions () {

        // Get the questions list we will be working from.
        Question[] questions;
        switch (npc.details.personality) {

            case PersonalityType.Helpful:
                questions = helpful;
                break;
            case PersonalityType.Annoying:
                questions = annoying;
                break;
            case PersonalityType.Stubborn:
                questions = stubborn;
                break;
            case PersonalityType.Awkward:
                questions = awkward;
                break;
            default:
                questions = helpful;
                break;

        }

        // Add appropriate question about target.
        switch (npc.details.targetKnowledge) {

            case 0: // NPC knows nothing.
                currentDialogueOptions.Add(questions[0]);
                break;
            case 1: // NPC has vague idea.
                currentDialogueOptions.Add(questions[1]);
                break;
            case 2: // NPC has good idea.
                currentDialogueOptions.Add(questions[2]);
                break;
            
        }

        // Add appropriate question about self.
        switch (npc.details.selfKnowledge) {

            case 0: // NPC knows nothing.
                currentDialogueOptions.Add(questions[3]);
                break;
            case 1: // NPC has vague idea.
                currentDialogueOptions.Add(questions[4]);
                break;
            case 2: // NPC has good idea.
                currentDialogueOptions.Add(questions[5]);
                break;
            
        }

        // Add random question
        currentDialogueOptions.Add(questions[6]);

        // Add appropriate accusation depending on whether NPC is target or not.
        if (npc != npcManager.target)
            currentDialogueOptions.Add(questions[7]);
        else
            currentDialogueOptions.Add(questions[8]);

        // Add leave conversation option.
        currentDialogueOptions.Add(questions[9]);

    }

    string SwapTag (string tag) {
        
        switch (tag) {

            // Tags to do with our target.
            case "targethair":
                if (!npcManager.knowsTargetHair) {
                    npcManager.knowsTargetHair = true;
                    newInfo.Activate();
                }
                return npcManager.target.details.GetHairColorText(false);
            case "targetpersonality":
                if (!npcManager.knowsTargetPersonality) {
                    npcManager.knowsTargetPersonality = true;
                    newInfo.Activate();
                }
                return npcManager.target.details.GetPersonalityText(false);
            case "targetskin":
                if (!npcManager.knowsTargetSkin) {
                    npcManager.knowsTargetSkin = true;
                    newInfo.Activate();
                }
                return npcManager.target.details.GetSkinToneText(false);
            case "targetage":
                if (!npcManager.knowsTargetAge) {
                    npcManager.knowsTargetAge = true;
                    newInfo.Activate();
                }
                return npcManager.target.details.GetAgeText(false);

            // Tags to do with the NPC we're speaking to.
            case "npchair":
                npc.details.knowsHair = true;
                return npc.details.GetHairColorText(false);
            case "npcpersonality":
                npc.details.knowsPersonality = true;
                return npc.details.GetPersonalityText(false);
            case "npcskin":
                npc.details.knowsSkin = true;
                return npc.details.GetSkinToneText(false);
            case "npcage":
                npc.details.knowsAge = true;
                return npc.details.GetAgeText(false);
            default:
                return "error";
        }

    }

    void UpdateQuestionText () {

        foreach (TextMeshProUGUI o in DialogueOptionTexts)
            Destroy(o.gameObject);

        DialogueOptionTexts.Clear();        

    
        for (int i = 0; i < currentDialogueOptions.Count; i++) {

            GameObject newOption = Instantiate(menuItemPrefab);
            newOption.transform.SetParent(DialogueOptionWindow);
            DialogueOptionTexts.Add(newOption.GetComponent<TextMeshProUGUI>());
            DialogueOptionTexts[DialogueOptionTexts.Count - 1].rectTransform.localScale = Vector3.one;
            DialogueOptionTexts[DialogueOptionTexts.Count - 1].text = currentDialogueOptions[i].simpleText;
            newOption.name = i.ToString();
            if (i == selectedIndex)
                DialogueOptionTexts[DialogueOptionTexts.Count - 1].color = Color.white;

        }

    }

    public void StartConversation (NPCController _npc) {

        isTalking = true;
        npc = _npc;
        buildDialogueOptions();
        DialogueTitle.text = npc.details.GetNameText(false);
        UpdateQuestionText();
        DialogueOptions.gameObject.SetActive(true);

    }

    void EndConversation () {

        npc = null;
        player.StopTalkingToNPC();
        isTalking = false;
        currentDialogueOptions.Clear();
        selectedIndex = 0;
        speechBox.text = "";
        DialogueOptions.gameObject.SetActive(false);

    }

}

[System.Serializable]
public class Question {

    public string simpleText;
    [SerializeField]
    private string[] answer;

    public string GetAnswerText () {
        return answer[Random.Range(0, answer.Length)];
    }

}
