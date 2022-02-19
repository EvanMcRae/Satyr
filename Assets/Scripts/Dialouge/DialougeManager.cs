using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialougeManager : MonoBehaviour
{

    public Text nameText;
    public Text DialogueText;

    private Queue<string> sentences;

    public static bool convoEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue (Dialouge dialouge)
    {
        convoEnded = false;
        // Debug.Log("Starting conversation with " + dialouge.name);

        nameText.text = dialouge.name;

        // clear any sentaces from a previous conversation
        sentences.Clear();

        foreach (string sentence in dialouge.sentaces)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
       // DialogueText.text = sentence;

    }

    
    IEnumerator TypeSentence (string sentence)
    {
        DialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        convoEnded = true;
        print("End of convo");
    }

}
