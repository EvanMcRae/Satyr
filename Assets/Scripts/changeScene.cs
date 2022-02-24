using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class changeScene : MonoBehaviour
{
    public string scene;
    public string spawn;
    private Animator crossfade;

    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(LoadNextScene());
        }
    }
    IEnumerator LoadNextScene() {
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
        spawnManager.spawningAt = spawn;
    }

}
