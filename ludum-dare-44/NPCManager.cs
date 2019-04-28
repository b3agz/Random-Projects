using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCManager : MonoBehaviour {

    public GameObject npcPrefab;
    public List<NPCController> npcs = new List<NPCController>();

    public Transform player;
    public NPCController target;

    public EndScreenHandler endScreen;

    public int questionedNPCsLeft; // The amount of NPCs the player can question before the target gets spooked.
    
    // Keep track of what we know about the target so far.
    [HideInInspector]
    public bool knowsTargetName = false;
    [HideInInspector]
    public bool knowsTargetAge = false;
    [HideInInspector]
    public bool knowsTargetHair = false;
    [HideInInspector]
    public bool knowsTargetSkin = false;
    [HideInInspector]
    public bool knowsTargetPersonality = false;

    public void Init(int minNPCs) {

        int noOfNPCs = Random.Range(minNPCs, minNPCs + 5);

        for (int i = 0; i < noOfNPCs; i++) {

            GameObject newNPC = Instantiate(npcPrefab, new Vector3(Random.Range(-6.5f, 6.5f), 0f, -0.002f * i), Quaternion.identity);
            npcs.Add(newNPC.GetComponent<NPCController>());

            npcs[npcs.Count - 1].Init(i * 2, player);          

        }

        target = npcs[Random.Range(0, npcs.Count)];
        target.details.targetKnowledge = 0; // Target will never give away any information about the target as it will incriminate them.
        target.details.isTarget = true;

        questionedNPCsLeft = Random.Range(7, 13);

    }

    public string OutputKnownTargetDetails () {

        string buffer = "";

        if (knowsTargetName)
            buffer += "NAME - " + target.details.GetNameText(false) + "\n";
        else
            buffer += "NAME - unknown\n";

        if (knowsTargetAge)
            buffer += "AGE - " + target.details.GetAgeText(false) + "\n";
        else
            buffer += "AGE - unknown\n";

        if (knowsTargetHair)
            buffer += "HAIR - " +  target.details.GetHairColorText(false) + "\n";
        else
            buffer += "HAIR - unknown\n";

        if (knowsTargetSkin)
            buffer += "SKIN - " + target.details.GetSkinToneText(false) + "\n";
        else
            buffer += "SKIN - unknown\n";

        if (knowsTargetPersonality)
            buffer += "PERSONALITY - " + target.details.GetPersonalityText(false);
        else
            buffer += "PERSONALITY - unknown";

        return buffer;

    }

    public void NPCAsked () {

        questionedNPCsLeft--;

        if (questionedNPCsLeft < 1) {

            GameOver();

        }
    }

    public void FoundThem () {

        endScreen.DoEndScreen("Target Found", "You got your man. Or woman. Or robot...look you got them alright.");

    }

    public void GameOver () {

        endScreen.DoEndScreen("Game Over", "Your target got spooked and ran away.");

    }

    public void PlayerExit () {

        endScreen.DoEndScreen("Returning to Menu", "I'd code a proper in-game pause menu but I ran out of time ;)");

    }

    public void Reset() {

        foreach (NPCController npc in npcs)
            Destroy(npc.gameObject);

        npcs.Clear();
        target = null;
    }

}