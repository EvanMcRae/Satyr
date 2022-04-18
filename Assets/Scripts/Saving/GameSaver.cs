using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaver : MonoBehaviour
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
        if (!Player.controller.dead && !Player.controller.resetting && !loading) {
            SaveData data = new SaveData();
            data.SetPlayer(Player.instance);
            data.SetOptions(AudioManager.instance);
            var dataToSave = JsonUtility.ToJson(data);
            saveSystem.SaveData(dataToSave);
        }
    }

    public void LoadGame()
    {
        if (!loading && !changeScene.changingScene && !Statue.cutscening) {
            if (Player.controller == null) {
                StartCoroutine(LoadSaveFile()); 
            } else if (!Player.controller.resetting && !Player.controller.dead) {
                StartCoroutine(LoadSaveFile());
            }
        }
            
    }

    IEnumerator LoadSaveFile()
    {
        loading = true;
        string dataToLoad = "";
        dataToLoad = saveSystem.LoadData();
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("start");
            yield return new WaitForSeconds(0.85f);
            Clear();
            SaveData data = JsonUtility.FromJson<SaveData>(dataToLoad);
            SceneManager.LoadSceneAsync(data.player.spawnpoint.scene);
            var newPlayer = Instantiate(prefab);
            newPlayer.GetComponent<Rigidbody2D>().simulated = false;
            newPlayer.transform.position = data.player.spawnpoint.position;

            // multi value transfers
            data.player.controller.SetValues(newPlayer);
            data.player.inventory.SetValues(newPlayer);
            data.player.health.SetValues(newPlayer);
            data.player.attack.SetValues(newPlayer);
            data.player.spawnpoint.SetValues(newPlayer);
            data.options.SetValues();

            // single value transfers
            newPlayer.GetComponent<PlayerMovement>().runSpeed = data.player.movement.runSpeed;
            newPlayer.GetComponent<Cordyceps>().count = data.player.cordyceps.count;
            
            Player.instance = newPlayer;
            Player.controller = newPlayer.GetComponent<Player>();
            // TODO this line may be either broken or unnecessary :I
            Player.controller.camTarget = GameObject.FindGameObjectWithTag("CamTarget").transform;
        }
    }

    [Serializable]
    public class SaveData
    {
        public PlayerSerialization player;
        public OptionsSerialization options;

        public void SetPlayer(GameObject playerObj) {
            player = new PlayerSerialization(playerObj);
        }
        public void SetOptions(AudioManager am) {
            options = new OptionsSerialization(am);
        }
    }
}