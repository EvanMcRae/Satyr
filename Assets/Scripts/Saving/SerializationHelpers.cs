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
    public SpawnpointSerialization spawnpoint;

    public PlayerSerialization(GameObject playerObj)
    {
        controller = new ControllerSerialization(playerObj.GetComponent<Player>());
        inventory = new InventorySerialization(playerObj.GetComponent<playerInventory>());
        health = new HealthSerialization(playerObj.GetComponent<health>());
        attack = new AttackSerialization(playerObj.GetComponent<Attack>());
        movement = new MovementSerialization(playerObj.GetComponent<PlayerMovement>());
        cordyceps = new CordycepsSerialization(playerObj.GetComponent<Cordyceps>());
        spawnpoint = new SpawnpointSerialization(playerObj.GetComponent<Spawnpoint>());
    }

}

[Serializable]
public class ControllerSerialization
{
    public bool m_Grounded, wallSlide_Unlocked, doubleJump_Unlocked, specialAttack_Unlocked, invincible, isJumping, explorer, initialFall, reya;
    public float stunDuration, iFrames, lastOnLand, jumpCooldown, life;

    public ControllerSerialization(Player controller)
    {
        m_Grounded = controller.m_Grounded;
        wallSlide_Unlocked = controller.wallSlide_Unlocked;
        doubleJump_Unlocked = controller.doubleJump_Unlocked;
        specialAttack_Unlocked = controller.specialAttack_Unlocked;
        invincible = controller.invincible;
        isJumping = controller.isJumping;
        explorer = controller.explorer;
        initialFall = controller.initialFall;
        reya = controller.reya;
        life = controller.life;
    }

    public void SetValues(GameObject playerObj)
    {
        playerObj.GetComponent<Player>().m_Grounded = m_Grounded;
        playerObj.GetComponent<Player>().wallSlide_Unlocked = wallSlide_Unlocked;
        playerObj.GetComponent<Player>().doubleJump_Unlocked = doubleJump_Unlocked;
        playerObj.GetComponent<Player>().specialAttack_Unlocked = specialAttack_Unlocked;
        playerObj.GetComponent<Player>().invincible = invincible;
        playerObj.GetComponent<Player>().isJumping = isJumping;
        playerObj.GetComponent<Player>().explorer = explorer;
        playerObj.GetComponent<Player>().initialFall = initialFall;
        playerObj.GetComponent<Player>().reya = reya;
        playerObj.GetComponent<Player>().life = life;
    }
}

[Serializable]
public class InventorySerialization
{
    public List<ItemSerialization> items_added;

    public InventorySerialization(playerInventory inventory)
    {
        items_added = new List<ItemSerialization>();
        foreach (Item item in inventory.items_added)
        {
            items_added.Add(new ItemSerialization(item));
        }
    }

    public void SetValues(GameObject playerObj)
    {
        playerObj.GetComponent<playerInventory>().items_added.Clear();
        foreach (ItemSerialization item in items_added)
        {
            playerObj.GetComponent<playerInventory>().items_added.Add(item.GetValue());
        }
        playerObj.GetComponent<playerInventory>().updateInventory();
    }
}

[Serializable]
public class ItemSerialization
{
    public string itemID;
    public int itemCount;

    public ItemSerialization(Item item)
    {
        itemID = item.itemID;
        itemCount = item.itemCount;
    }

    public Item GetValue()
    {
        Item newItem = Database.GetItemByID(itemID);
        if (newItem != null)
            newItem.itemCount = itemCount;
        return newItem;
    }
}

[Serializable]
public class HealthSerialization
{
    public int playerHealth, numberOfHearts;

    public HealthSerialization(health health)
    {
        playerHealth = health.playerHealth;
        numberOfHearts = health.numberOfHearts;
    }

    public void SetValues(GameObject playerObj)
    {
        playerObj.GetComponent<health>().numberOfHearts = numberOfHearts;
        playerObj.GetComponent<health>().playerHealth = numberOfHearts;
    }
}

[Serializable]
public class AttackSerialization
{
    public float dmgValue, specialCooldown, specialMaxCooldown;
    public bool shooting_Unlocked;

    public AttackSerialization(Attack attack)
    {
        dmgValue = attack.dmgValue;
        shooting_Unlocked = attack.shooting_Unlocked;
        specialCooldown = attack.specialCooldown;
        specialMaxCooldown = attack.specialMaxCooldown;
    }

    public void SetValues(GameObject playerObj)
    {
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
    public bool dash_Unlocked;

    public MovementSerialization(PlayerMovement movement)
    {
        runSpeed = movement.runSpeed;
        dash_Unlocked = movement.dash_Unlocked;
    }

    public void SetValues(GameObject playerObj)
    {
        playerObj.GetComponent<PlayerMovement>().runSpeed = runSpeed;
        playerObj.GetComponent<PlayerMovement>().dash_Unlocked = dash_Unlocked;
    }
}

[Serializable]
public class CordycepsSerialization
{
    public int count;

    public CordycepsSerialization(Cordyceps cordyceps)
    {
        count = cordyceps.count;
    }

    public void SetValues(GameObject playerObj)
    {
        playerObj.GetComponent<Cordyceps>().count = count;
    }
}

[Serializable]
public class SpawnpointSerialization
{
    public string scene;
    public Vector2 position;
    public List<int> statuesUsed;

    public SpawnpointSerialization(Spawnpoint spawnpoint)
    {
        scene = spawnpoint.scene;
        position = spawnpoint.position;
        statuesUsed = spawnpoint.statuesUsed;
    }

    public void SetValues(GameObject playerObj)
    {
        playerObj.GetComponent<Spawnpoint>().scene = scene;
        playerObj.GetComponent<Spawnpoint>().position = position;
        playerObj.GetComponent<Spawnpoint>().statuesUsed = statuesUsed;
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
public class OptionsSerialization
{
    public bool musicMute;
    public float musicVolume;

    public OptionsSerialization(AudioManager am)
    {
        musicMute = am.mute;
        musicVolume = am.volume;
    }

    public void SetValues()
    {
        AudioManager.instance.mute = musicMute;
        AudioManager.instance.volume = musicVolume;
    }
}