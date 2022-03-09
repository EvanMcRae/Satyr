using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaver : MonoBehaviour
{
    public SaveSystem saveSystem;

    public GameObject prefab;

    public void Clear()
    {
        Destroy(CharacterController2D.instance.gameObject);
        CharacterController2D.instance = null;
    }

    // public void SpawnPrefab()
    // {
    //     var position = Random.insideUnitSphere * 5;
    //     createdPrefabs.Add(Instantiate(prefab, position, Quaternion.identity));
    // }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        data.player = CharacterController2D.instance;
        data.SetPosition(CharacterController2D.instance.gameObject.transform.position);
        var dataToSave = JsonUtility.ToJson(data);
        saveSystem.SaveData(dataToSave);
    }

    public void LoadGame()
    {
        StartCoroutine(LoadSaveFile());
    }

    IEnumerator LoadSaveFile()
    {
        string dataToLoad = "";
        dataToLoad = saveSystem.LoadData();
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("start");
            yield return new WaitForSeconds(1f);
            SaveData data = JsonUtility.FromJson<SaveData>(dataToLoad);
            SceneManager.LoadSceneAsync(data.sceneIndex);
            CharacterController2D.instance.gameObject.transform.position = data.positionData.GetValue();
            CharacterController2D.instance = data.player;
        }
    }

    [Serializable]
    public class SaveData
    {
        public Vector2Serialization positionData;
        public int sceneIndex;
        public CharacterController2D player;

        public void SetPosition(Vector2 position)
        {
            positionData = new Vector2Serialization(position);
        }
    }


}