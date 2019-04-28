using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {


    public Material spriteMat;

    public float defaultAnimationSpeed = 0.15f;
    public CustomAnimation[] animations;
    private int currentAnimationIndex = 0;
    private int currentSpriteIndex;
    private float timer = 0;
    private bool isPlaying = true;
    private string lastRequest;

    string followOnAnimation = "";

    private SpriteRenderer hair;
    private SpriteRenderer skin;
    private SpriteRenderer body;
    private SpriteRenderer arms;
    private SpriteRenderer legs;
    private SpriteRenderer feet;

    private float currentTimer;

    void Awake() {

        isPlaying = false;

        for (int i = 0; i < 6; i++) {

            GameObject bodyPart = new GameObject();
            bodyPart.transform.position = transform.position;
            bodyPart.transform.SetParent(transform);
            switch (i) {

                case 0:
                    hair = bodyPart.AddComponent<SpriteRenderer>();
                    hair.material = spriteMat;
                    bodyPart.name = "hair";
                    hair.sortingOrder = 1;
                    break;
                case 1:
                    skin = bodyPart.AddComponent<SpriteRenderer>();
                    skin.material = spriteMat;
                    bodyPart.name = "skin";
                    break;
                case 2:
                    body = bodyPart.AddComponent<SpriteRenderer>();
                    body.material = spriteMat;
                    bodyPart.name = "body";
                    break;
                case 3:
                    arms = bodyPart.AddComponent<SpriteRenderer>();
                    arms.material = spriteMat;
                    bodyPart.name = "arms";
                    break;
                case 4:
                    legs = bodyPart.AddComponent<SpriteRenderer>();
                    legs.material = spriteMat;
                    bodyPart.name = "legs";
                    break;
                case 5:
                    feet = bodyPart.AddComponent<SpriteRenderer>();
                    feet.material = spriteMat;
                    bodyPart.name = "feet";
                    break;

            }

        }

        body.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        arms.color = body.color;
        legs.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        timer = Random.Range(-0.05f, 0.15f);

    }

    void Update() {
        
        if (timer > currentTimer) {

            timer = 0;
            UpdateAnimation();

        }

        timer += Time.deltaTime;

    }

    void UpdateAnimation () {

        if (!isPlaying)
            return;

        hair.sprite = animations[currentAnimationIndex].frames[currentSpriteIndex].hair;
        skin.sprite = animations[currentAnimationIndex].frames[currentSpriteIndex].skin;
        body.sprite = animations[currentAnimationIndex].frames[currentSpriteIndex].body;
        arms.sprite = animations[currentAnimationIndex].frames[currentSpriteIndex].arms;
        legs.sprite = animations[currentAnimationIndex].frames[currentSpriteIndex].legs;
        feet.sprite = animations[currentAnimationIndex].frames[currentSpriteIndex].feet;

        if (animations[currentAnimationIndex].frames[currentSpriteIndex].frameLength == 0)
            currentTimer = defaultAnimationSpeed;
        else
            currentTimer = animations[currentAnimationIndex].frames[currentSpriteIndex].frameLength;

        currentSpriteIndex++;

        if (currentSpriteIndex > animations[currentAnimationIndex].frames.Length - 1) {

            currentSpriteIndex = 0;
            if (followOnAnimation != "")
                SelectAnimation(followOnAnimation, true);                

        }

    }

    public void SetNextAnimation (string nextAnim) {

        followOnAnimation = nextAnim;

    }

    public void SelectAnimation (string name, bool autoplay) {

        isPlaying = autoplay;
        if (name == lastRequest)
            return;

        for (int i = 0; i < animations.Length; i++) {

            if (name == animations[i].AnimationName) {
                lastRequest = name;
                currentSpriteIndex = 0;
                currentAnimationIndex = i;
                UpdateAnimation();
                return;

            }

        }

        Debug.Log("Invalid animation selection. No animation by the name of \"" + name + "\".");

    }

    public void Play () {

        isPlaying = true;

    }

    public void Stop () {

        isPlaying = false;

    }

    public void SetSortingOrder (int order) {

        hair.sortingOrder = order + 1;
        body.sortingOrder = order;
        legs.sortingOrder = order;
        arms.sortingOrder = order;
        feet.sortingOrder = order;
        skin.sortingOrder = order;

    }

    public void SetColours (Color _hair, Color _skin) {

        hair.color = _hair;
        skin.color = _skin;

    }

}

[System.Serializable]
public class CustomAnimation {

    public string AnimationName;
    public AnimationFrame[] frames;

}

[System.Serializable]
public class AnimationFrame {

    public Sprite hair;
    public Sprite skin;
    public Sprite body;
    public Sprite arms;
    public Sprite legs;
    public Sprite feet;

    public float frameLength;

}
