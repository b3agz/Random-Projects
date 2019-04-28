using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    [Range(0, 1f)]
    public float speed = 0.15f;

    public NPCDetails details;

    public int helpfulness;

    public AnimationController animController;
    Vector3 direction;
    float targetX;
    float originX;
    bool isWalking = true;
    bool isTalking = false;
    Transform playerTransform;

    public void Init(int sortingOrder, Transform _playerTransform) {

        playerTransform = _playerTransform;

        details = new NPCDetails(GenerateName());
        
        transform.name = details.name;
        originX = transform.position.x;

        // Give NPC a random starting direction.
        direction = Vector3.right;
        if (Random.Range(0, 2) == 1)
            ChangeDirection();

        animController.SetSortingOrder (sortingOrder);
        animController.SetColours(details.hair, details.skin);

        // Give NPC a random speed tweek for variation.
        float speedTweek = Random.Range(-0.05f, 0.05f);
        speed += speedTweek;
        animController.defaultAnimationSpeed -= speedTweek;
        animController.Play();

        SetNewTarget();

    }

    void Update() {

        if (isWalking) {
            
            // Move NPC in direction we're currently facing.
            transform.position += direction * Time.deltaTime * speed;

            // If our target is to the left but we're facing to the right...
            if (targetX < transform.position.x && direction.x > 0)
                ChangeDirection();
            // ...or if target is to the right and we're facing left...
            else if (targetX > transform.position.x && direction.x < 0)
                ChangeDirection();
            // ...or we're close enough to our destination.
            else if (Mathf.Abs(targetX - transform.position.x) < 0.1f)
                StartCoroutine("StandAround");

        }

    }

    void ChangeDirection () {

        if (direction.x > 0)
            direction = Vector3.left;
        else
            direction = Vector3.right;

        transform.Rotate(new Vector3(0, 180f, 0));

    }

    IEnumerator StandAround () {

        float standingTime = Random.Range(0.2f, 3f);
        animController.SelectAnimation("Idle", true);
        isWalking = false;

        while (standingTime > 0) {

            yield return null;
            standingTime -= Time.deltaTime;

        }

        SetNewTarget();

    }

    void SetNewTarget () {

        targetX = originX + Random.Range(-1f, 1f); // Get a new location to walk to within a certain range of this npc's origin.
        animController.SelectAnimation("Walk", true);
        isWalking = true;

    }

    public void TalkToPlayer () {

        isWalking = false;
        isTalking = true;
        animController.SelectAnimation("Idle", true);

        if (playerTransform.position.x < transform.position.x && direction.x > 0)
            ChangeDirection();
        else if (playerTransform.position.x > transform.position.x && direction.x < 0)
            ChangeDirection();

        // Stop Coroutines so NPCs who were standing still when we started talking to them don't just start walking off at the end of the routine.
        StopAllCoroutines();

    }

    public void StopTalkingToPlayer () {

        isTalking = false;
        SetNewTarget();

    }

    string GenerateName () {

        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string[] surnames = new string[] {"Smith", "Anderson", "Lomas", "Bullock", "Santorino", "Parker", "Keighley", "Tunnard", "Jones", "Goldstein", "Ripley", "Start" };

        char initial = alphabet[Random.Range(0, alphabet.Length - 1)];

        return alphabet[Random.Range(0, alphabet.Length - 1)] + ". " + surnames[Random.Range(0, surnames.Length - 1)];

    }

    void OnDestroy () {

        animController.enabled = false;

    }

 }

public enum PersonalityType {

    Helpful,
    Annoying,
    Stubborn,
    Awkward

}

[System.Serializable]
public class NPCDetails {

    public string name;
    public int age;
    public PersonalityType personality;
    int hairIndex;
    int skinIndex;
    int questionsAsked = 0;
    int questionsToUnlockPersonality; // The number of questions player must ask this NPC before they learn their personality type.

    // These values determine how much this NPC will let on when asked. 0 = nothing, 1 = vague answers, 2 = good answers.
    public int targetKnowledge;
    public int selfKnowledge;

    Color[] hairColors = new Color[] { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.grey, Color.red, Color.white, Color.yellow };
    Color[] skinTones = new Color[] { new Color(1f, 0.7877f, 0.7877f), new Color (0.6886792f, 0.4682122f, 0.3410911f), new Color (0.2924528f, 0.1697767f, 0.04552332f), new Color (0.9056604f, 0.7758138f, 0.5596298f), new Color (0.5471698f, 0.5471698f, 0.5471698f) };

    public bool knowsName = false;
    public bool knowsAge = false;
    public bool knowsVagueAge = false;
    public bool knowsHair = false;
    public bool knowsSkin = false;
    public bool knowsPersonality = false;

    public bool isTarget = false;

    public NPCDetails (string _name) {

        name = _name;
        age = Random.Range(18, 65);
        hairIndex = Random.Range(0, hairColors.Length);
        skinIndex = Random.Range(0, skinTones.Length);
        SetPersonality();

        targetKnowledge = Random.Range(0, 3);
        selfKnowledge = Random.Range(0, 3);
        questionsToUnlockPersonality = Random.Range(2, 5);

    }

    public string OutputDetails () {

        string buffer = string.Format("NAME - {0} \nAGE - {1}\nHAIR - {2}\nSKIN - {3}\nPERSONALITY - {4}", GetNameText(true), GetAgeText(true), GetHairColorText(true), GetSkinToneText(true), GetPersonalityText(true));

        return buffer;

    }

    public void askedQuestion () {

        questionsAsked++;
        if (questionsAsked >= questionsToUnlockPersonality)
            knowsPersonality = true;
        
    }

    public string GetNameText (bool mask) {

        if (mask && !knowsName)
            return "???";
        else
            return name;


    }

    public string GetAgeText (bool mask) {

        if (mask && !knowsAge)
            return "???";
        else
            return age.ToString();

    }

    public Color hair {

        get { return hairColors[hairIndex]; }

    }

    public Color skin {

        get { return skinTones[skinIndex]; }

    }

    public string GetPersonalityText (bool mask) {

        if (mask && !knowsPersonality)
            return "???";

        switch (personality) {

            case PersonalityType.Helpful:
                return "helpful";
            case PersonalityType.Annoying:
                return "annoying";
            case PersonalityType.Stubborn:
                return "stubborn";
            case PersonalityType.Awkward:
                return "awkward";
            default:
                return "error";

        }

    }

    void SetPersonality () {

        switch (Random.Range(0, 4)) {

            case 0:
                personality = PersonalityType.Helpful;
                break;
            case 1:
                personality = PersonalityType.Annoying;
                break;
            case 2:
                personality = PersonalityType.Stubborn;
                break;
            case 3:
                personality = PersonalityType.Awkward;
                break;

        }

    }

    public string GetHairColorText(bool mask) {

        if (mask && !knowsHair)
            return "???";

        switch (hairIndex) {

            case 0:
                return "black";
            case 1:
                return "blue";
            case 2:
                return "cyan";
            case 3:
                return "green";
            case 4:
                return "magenta";
            case 5:
                return "grey";
            case 6:
                return "red";
            case 7:
                return "white";
            case 8:
                return "yellow";
            default:
                return "error";

        }

    }

    public string GetSkinToneText(bool mask) {

        if (mask && !knowsSkin)
            return "???";

        switch (skinIndex) {

            case 0:
                return "light";
            case 1:
                return "bronze";
            case 2:
                return "dark";
            case 3:
                return "olive";
            case 4:
                return "metal";
            default:
                return "error";

        }

    }

}
