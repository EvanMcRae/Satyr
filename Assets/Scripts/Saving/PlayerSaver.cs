using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaver : MonoBehaviour
{
    public SaveSystem saveSystem;
    public GameObject prefab;
    public static bool loading = false;

    public void Clear()
    {
        Destroy(Player.instance);
        Player.instance = null;
        Player.controller = null;
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        data.SetPlayer(Player.instance);
        var dataToSave = JsonUtility.ToJson(data);
        saveSystem.SaveData(dataToSave);
    }

    public void LoadGame()
    {
        StartCoroutine(LoadSaveFile());
    }

    IEnumerator LoadSaveFile()
    {
        loading = true;
        string dataToLoad = "";
        dataToLoad = saveSystem.LoadData();
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("start");
            yield return new WaitForSeconds(0.9f);
            Clear();
            SaveData data = JsonUtility.FromJson<SaveData>(dataToLoad);
            SceneManager.LoadSceneAsync(data.sceneIndex);
            var newPlayer = Instantiate(prefab);
            newPlayer.transform.position = data.player.position.GetValue();

            // multi value transfers
            data.player.controller.SetValues(newPlayer);
            data.player.inventory.SetValues(newPlayer);
            data.player.health.SetValues(newPlayer);
            data.player.attack.SetValues(newPlayer);

            // single value transfers
            newPlayer.GetComponent<PlayerMovement>().runSpeed = data.player.movement.runSpeed;
            newPlayer.GetComponent<Cordyceps>().count = data.player.cordyceps.count;
            
            Player.instance = newPlayer;
            Player.controller = newPlayer.GetComponent<Player>();
            Player.camTarget = GameObject.FindGameObjectWithTag("CamTarget").transform;
        }
    }

    [Serializable]
    public class SaveData
    {
        public PlayerSerialization player;
        public int sceneIndex;
        public void SetPlayer(GameObject playerObj) {
            player = new PlayerSerialization(playerObj);
        }
    }


}