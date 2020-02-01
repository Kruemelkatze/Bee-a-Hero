using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private float startDelay;
    [SerializeField] private float timeBetweenLetters;

    public static DialogueController Instance;

    private Queue<string> sentences;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
        Dialogue nextDialogue = GameController.Instance.GetNextDialogue();
        if (nextDialogue != null)
        {
            StartDialogue(nextDialogue);   
        }
        else
        {
            animator.SetBool("IsOpen", false);
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        
        foreach (char item in sentence.ToCharArray())
        {
            dialogueText.text += item;
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }

    private IEnumerator StartDialogueCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        DisplayNextSentence();
    }

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        image.sprite = dialogue.image;
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

        dialogueText.text = "";
        StartCoroutine(StartDialogueCoroutine());
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
        StartCoroutine(TypeSentence(sentence));
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}