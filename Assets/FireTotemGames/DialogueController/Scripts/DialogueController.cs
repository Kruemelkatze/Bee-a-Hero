using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private float startDelay;
    [SerializeField] private float timeBetweenLetters;

    public static DialogueController instance;

    private Queue<string> sentences;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        sentences = new Queue<string>();
    }

    private void Update()
    {

    }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }

    private IEnumerator TypeSentence(string sentence, bool isFirstSentence = false)
    {
        dialogueText.text = "";

        if (isFirstSentence == true)
        {
            yield return new WaitForSeconds(startDelay);
        }

        foreach (char item in sentence.ToCharArray())
        {
            dialogueText.text += item;
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        if (sentences == null)
        {
            sentences = new Queue<string>();
        }
        sentences.Clear();

        foreach (string item in dialogue.sentences)
        {
            sentences.Enqueue(item);
        }

        DisplayNextSentence(true);
    }

    public void DisplayNextSentence(bool isFirstSentence = false)
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, isFirstSentence));
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}