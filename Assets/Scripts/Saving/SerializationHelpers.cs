using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSerialization
{
    public ControllerSerialization controller;
    public InventorySerialization inventory;
    public HealthSerialization health;
    public AttackSerialization attack;
    public MovementSerialization movement;
    public CordycepsSerialization cordyceps;
    public Vector2Serialization position;

    public PlayerSerialization(GameObject playerObj) {
        controller = new ControllerSerialization(playerObj.GetComponent<Player>());
        inventory = new InventorySerialization(playerObj.GetComponent<playerInventory>());
        health = new HealthSerialization(playerObj.GetComponent<health>());
        attack = new AttackSerialization(playerObj.GetComponent<Attack>());
        movement = new MovementSerialization(playerObj.GetComponent<PlayerMovement>());
        cordyceps = new CordycepsSerialization(playerObj.GetComponent<Cordyceps>());
        position = new Vector2Serialization(playerObj.transform.position);
    }

}

[Serializable]
public class ControllerSerialization
{
    public bool m_Grounded, wallSlide_Unlocked, doubleJump_Unlocked, canDoubleJump, canDash, invincible, isJumping, explorer;
    public float stunDuration, iFrames, lastOnLand, jumpCooldown, life;

    public ControllerSerialization(Player controller) {
        m_Grounded = controller.m_Grounded;
        wallSlide_Unlocked = controller.wallSlide_Unlocked;
        doubleJump_Unlocked = controller.doubleJump_Unlocked;
        canDoubleJump = controller.canDoubleJump;
        canDash = controller.canDash;
        invincible = controller.invincible;
        isJumping = controller.isJumping;
        explorer = controller.explorer;
        life = controller.life;
    }
    
    public void SetValues(GameObject playerObj) {
        playerObj.GetComponent<Player>().m_Grounded = m_Grounded;
        playerObj.GetComponent<Player>().wallSlide_Unlocked = wallSlide_Unlocked;
        playerObj.GetComponent<Player>().doubleJump_Unlocked = doubleJump_Unlocked;
        playerObj.GetComponent<Player>().canDoubleJump = canDoubleJump;
        playerObj.GetComponent<Player>().canDash = canDash;
        playerObj.GetComponent<Player>().invincible = invincible;
        playerObj.GetComponent<Player>().isJumping = isJumping;
        playerObj.GetComponent<Player>().explorer = explorer;
        playerObj.GetComponent<Player>().life = life;
    }
}

[Serializable]
public class InventorySerialization
{
    public List<ItemSerialization> items_added;
    
    public InventorySerialization(playerInventory inventory) {
        items_added = new List<ItemSerialization>();
        foreach (Item item in inventory.items_added) {
            items_added.Add(new ItemSerialization(item));
        }
    }

    public void SetValues(GameObject playerObj) {
        playerObj.GetComponent<playerInventory>().items_added.Clear();
        foreach (ItemSerialization item in items_added) {
            playerObj.GetComponent<playerInventory>().items_added.Add(item.GetValue());
        }
        playerObj.GetComponent<playerInventory>().updateInventory();
    }
}

// TODO: FIGURE OUT HOW TO SERIALIZE SPRITES!!
// maybe there's a better organizational strategy for serializing items as prefabs or something
// better yet: associate sprites WITH ID!!! banger idea
[Serializable]
public class ItemSerialization
{
    public string itemID, itemName, itemDescription;
    public int itemCount, maxStack;

    public ItemSerialization(Item item) {
        itemID = item.itemID;
        itemName = item.itemName;
        itemDescription = item.itemDescription;
        itemCount = item.itemCount;
        maxStack = item.maxStack;
    }

    public Item GetValue() {
        Item newItem = new Item();
        newItem.itemID = itemID;
        newItem.itemName = itemName;
        newItem.itemDescription = itemDescription;
        newItem.itemCount = itemCount;
        newItem.maxStack = maxStack;
        return newItem;
    }
}

[Serializable]
public class HealthSerialization
{
    public int playerHealth, numberOfHearts;

    public HealthSerialization(health health) {
        playerHealth = health.playerHealth;
        numberOfHearts = health.numberOfHearts;
    }

    public void SetValues(GameObject playerObj) {
        playerObj.GetComponent<health>().playerHealth = playerHealth;
        playerObj.GetComponent<health>().numberOfHearts = numberOfHearts;
    }
}

[Serializable]
public class AttackSerialization
{
    public float dmgValue, specialCooldown, specialMaxCooldown;
    public bool shooting_Unlocked;

    public AttackSerialization(Attack attack) {
        dmgValue = attack.dmgValue;
        shooting_Unlocked = attack.shooting_Unlocked;
        specialCooldown = attack.specialCooldown;
        specialMaxCooldown = attack.specialMaxCooldown;
    }

    public void SetValues(GameObject playerObj) {
        playerObj.GetComponent<Attack>().dmgValue = dmgValue;
        playerObj.GetComponent<Attack>().shooting_Unlocked = shooting_Unlocked;
        playerObj.GetComponent<Attack>().specialCooldown = specialCooldown;
        playerObj.GetComponent<Attack>().specialMaxCooldown = specialMaxCooldown;
    }
}

[Serializable]
public class MovementSerialization
{
    public float runSpeed;

    public MovementSerialization(PlayerMovement movement)
    {
        runSpeed = movement.runSpeed;
    }
}

[Serializable]
public class CordycepsSerialization
{
    public int count;

    public CordycepsSerialization(Cordyceps cordyceps) {
        count = cordyceps.count;
    }
}


[Serializable]
public class Vector3Serialization
{
    public float x, y, z;

    public Vector3Serialization(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }

    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class Vector2Serialization
{
    public float x, y;

    public Vector2Serialization(Vector2 position)
    {
        x = position.x;
        y = position.y;
    }

    public Vector2 GetValue()
    {
        return new Vector2(x, y);
    }
}

[Serializable]
public class OptionsSerialization {
    public bool musicMute;
    public float musicVolume;

    public OptionsSerialization(AudioManager am) {
        musicMute = am.mute;
        musicVolume = am.volume;
    }

    public void SetValues() {
        AudioManager.instance.mute = musicMute;
        AudioManager.instance.volume = musicVolume;
    }
}