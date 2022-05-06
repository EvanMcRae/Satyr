using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialougeManager : MonoBehaviour
{

    public Text nameText;
    public Text DialogueText;

    public Queue<string> sentences;
    private string currentSentence;

    public static bool convoEnded = false;
    public static bool stillSpeaking = false;

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
        if (sentences != null)
            sentences.Clear();
        else {
            sentences = new Queue<string>();
        }
            
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

        currentSentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
       // DialogueText.text = sentence;

    }

    public void FinishSentence() {
        if (stillSpeaking)
        {
            StopAllCoroutines();
            DialogueText.text = currentSentence;
            stillSpeaking = false;
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        stillSpeaking = true;
        DialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;
            yield return null;
        }
        stillSpeaking = false;
    }

    void EndDialogue()
    {
        convoEnded = true;
        stillSpeaking = false;
        print("End of convo");
    }

}
