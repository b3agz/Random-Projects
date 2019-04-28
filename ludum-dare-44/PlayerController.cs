using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour {

    public float speed;

    public bool isLocked = true;

    public Sprite idle;
    public Sprite glide;
    public GameObject scanner;
    NPCController scannedNPC;
    public Transform cursor;

    public DetailWindow targetInfoPanel;
    public DetailWindow scannedInfoPanel;

    SpriteRenderer sR;
    CameraController _camera;
    DialogueHandler dialogueHandler;
    public NPCManager npcManager;

    Vector3 direction;
    bool isTalking = false;

    float targetX;

    public List<NPCController> npcsInRange = new List<NPCController>();

    private void Start() {

        sR = GetComponent<SpriteRenderer>();
        _camera = Camera.main.transform.GetComponent<CameraController>();
        dialogueHandler = GameObject.Find("Game").GetComponent<DialogueHandler>();
        
        direction = Vector3.right;

        Cursor.visible = false;

    }

    private void Update() {

        cursor.position = Input.mousePosition;

        // If we're not talking or scanning anyone...
        if (isLocked || (!isTalking && scannedNPC == null)) {

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {

                if (direction.x < 0)
                    ChangeDirection();
                direction = Vector3.right;
                sR.sprite = glide;
                Vector3 move = transform.position + direction * speed * Time.deltaTime;
                if (move.x > 8.4f)
                    move.x = 8.4f;
                transform.position = Vector3.MoveTowards(transform.position, move, Time.deltaTime);

            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {

                if (direction.x > 0)
                    ChangeDirection();
                sR.sprite = glide;
                Vector3 move = transform.position + direction * speed * Time.deltaTime;
                if (move.x < -8.4f)
                    move.x = -8.4f;
                transform.position = Vector3.MoveTowards(transform.position, move, Time.deltaTime);

            }
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
                sR.sprite = idle;

            if (npcsInRange.Count > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))) {
                TalkToNPC();
            }

            if (Input.GetMouseButtonDown(0)) {

                RaycastHit hit;
                Ray ray = _camera.cam.ScreenPointToRay(cursor.position);
        
                if (Physics.Raycast(ray, out hit)) {

                    if (hit.transform.tag == "NPC") {
                        scannedNPC = hit.transform.GetComponent<NPCController>();
                        scanner.transform.SetParent(hit.transform);
                        scanner.transform.localPosition = Vector3.zero;
                        scanner.SetActive(true);
                        _camera.Follow(hit.transform);

                        // By looking at an NPC we know their hair and skin colour, so set those details to true.
                        scannedNPC.details.knowsHair = true;
                        scannedNPC.details.knowsSkin = true;

                        targetInfoPanel.gameObject.SetActive(true);
                        targetInfoPanel.UpdateWindowContent(npcManager.OutputKnownTargetDetails());
                        scannedInfoPanel.gameObject.SetActive(true);
                        scannedInfoPanel.UpdateWindowContent(scannedNPC.details.OutputDetails());

                    }
                }

            }
        // Else if we're talking to someone.
        } else if (isTalking) {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime * speed);

        // Else if we're scanning someone.
        } else {
            if (Input.anyKeyDown) {
                _camera.UnFollow();
                scannedNPC = null;
                scanner.SetActive(false);
                scanner.transform.SetParent(null);
                targetInfoPanel.gameObject.SetActive(false);
                scannedInfoPanel.gameObject.SetActive(false);
            }
        }
    }

    void ChangeDirection () {

        if (direction.x > 0)
            direction = Vector3.left;
        else
            direction = Vector3.right;

        transform.Rotate(new Vector3(0, 180f, 0));

    }

    void TalkToNPC () {

        npcsInRange[0].TalkToPlayer();

        // Talking to an NPC reveals their name (or the one they're using at least) so if we've started conversation, tell the details class that we know their name.
        npcsInRange[0].details.knowsName = true;
        // And if we're this close, we know their hair and skin colour too.
        npcsInRange[0].details.knowsHair = true;
        npcsInRange[0].details.knowsSkin = true;

        Transform npcTransform = npcsInRange[0].transform;

        if (npcTransform.position.x < transform.position.x) {

            if (direction.x > 0)
                ChangeDirection();
            targetX = npcTransform.position.x + 0.2f;
            _camera.Focus(npcTransform.position.x + 0.1f);

        }  else if (npcTransform.position.x > transform.position.x) {

            if (direction.x < 0)
                ChangeDirection();
            targetX = npcTransform.position.x - 0.2f;
            _camera.Focus(npcTransform.position.x - 0.1f);

        }

        dialogueHandler.StartConversation(npcsInRange[0]);
        isTalking = true;

    }

    public void StopTalkingToNPC () {

        npcsInRange[0].StopTalkingToPlayer();
        npcsInRange.Clear();
        isTalking = false;
        _camera.UnFocus();

    }

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "NPC" && !isTalking)
            npcsInRange.Add(other.GetComponent<NPCController>());

    }

    private void OnTriggerExit(Collider other) {
       
        if (other.tag == "NPC" && !isTalking)
            npcsInRange.Remove(other.GetComponent<NPCController>());

    }

}
