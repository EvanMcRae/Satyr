using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemPickup : MonoBehaviour
{
    public Item thisItem;
    Item item;
    public bool playerIsInRange = false;
    string labelText = "";
    public GameObject trigger;
    public string method = "";

    public float speed;
    public float verticalSpeed;
    public float maxRotation;
    public float x = 0;
    public float y = 0;
    public GameObject InfoBox, InfoText;
    private bool pickedup;
    
    // Start is called before the first frame update
    void Start()
    {
        item = thisItem;
        if (item.itemID == "10" && Player.controller.reya) Destroy(gameObject);
        if (item.itemID == "11" && Player.controller.froggy) Destroy(gameObject);
        if (item.itemID == "7" && Player.instance.GetComponent<Attack>().shooting_Unlocked) Destroy(gameObject);
        if (item.itemID == "9" && Player.controller.wallSlide_Unlocked) Destroy(gameObject);
        if (item.itemID == "4" && Player.controller.doubleJump_Unlocked) Destroy(gameObject);
        if (item.itemID == "3" && Player.instance.GetComponent<PlayerMovement>().dash_Unlocked) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!pickedup && !PlayerMovement.paused && Input.GetKey(KeyCode.T) || Input.GetKeyDown("joystick button 1"))
        {
            if (playerIsInRange == true)
            {
                playerInventory inventory = Player.instance.GetComponent<playerInventory>();

                if (item.itemID == "4")
                {
                    Player.controller.doubleJump_Unlocked = true;
                    StartCoroutine(UnlockAbility("Double Jump"));
                }
                else if (item.itemID == "9")
                {
                    Player.controller.wallSlide_Unlocked = true;
                    StartCoroutine(UnlockAbility("Wall Slide"));
                }
                else if (item.itemID == "10")
                {
                    Player.controller.specialAttack_Unlocked = true;
                    StartCoroutine(UnlockAbility("Special Attack"));
                }
                else if (item.itemID == "3")
                {
                    Player.instance.GetComponent<PlayerMovement>().dash_Unlocked = true;
                    StartCoroutine(UnlockAbility("Dash"));
                }
                else if (item.itemID == "7")
                {
                    Player.instance.GetComponent<Attack>().shooting_Unlocked = true;
                    Player.instance.GetComponent<Attack>().canShoot = true;
                    StartCoroutine(UnlockAbility("Bow"));
                }
                else if (item.itemID == "11")
                {
                    Player.controller.froggy = true;
                }

                if (trigger != null)
                {
                    trigger.BroadcastMessage(method);
                }

                if (inventory.addToInventory(item))
                {
                    StartCoroutine(DestroyItem());
                }
            }
        }

        transform.rotation = Quaternion.Euler(0f, 0f, maxRotation * Mathf.Sin(Time.time * speed));

        transform.position = new Vector3(x, (.3f * Mathf.Sin(Time.time * verticalSpeed)) + y, 0f);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && InfoText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("textfade_hold"))
        {
            playerIsInRange = true;
            labelText = "Press B button or T key to pick up item";
            InfoText.GetComponent<Text>().text = labelText;
            InfoText.GetComponent<Animator>().SetTrigger("start");
            InfoBox.GetComponent<Animator>().SetTrigger("start");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
        InfoText.GetComponent<Animator>().SetTrigger("stop");
        InfoBox.GetComponent<Animator>().SetTrigger("stop");
    }

    IEnumerator UnlockAbility(string ability)
    {
        InfoText.GetComponent<Text>().text = "Unlocked " + ability + "!";
        yield return new WaitForSeconds(1.0f);
        InfoText.GetComponent<Animator>().SetTrigger("stop");
        InfoBox.GetComponent<Animator>().SetTrigger("stop");
    }

    IEnumerator DestroyItem()
    {
        pickedup = true;
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1.1f);
        Destroy(gameObject);
    }
}
