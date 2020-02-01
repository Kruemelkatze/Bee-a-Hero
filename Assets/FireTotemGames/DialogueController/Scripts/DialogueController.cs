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

    [SerializeField] private GameObject raycastStopPanel;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private float startDelay;
    [SerializeField] private float timeBetweenLetters;

    public static DialogueController Instance;

    private Queue<string> sentences;
    private bool firstDialogue;

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
        firstDialogue = true;
        sentences = new Queue<string>();
        raycastStopPanel.SetActive(false);
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
            raycastStopPanel.SetActive(false);
            animator.SetBool("IsOpen", false);
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        if (firstDialogue == true)
        {
            firstDialogue = false;
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
        raycastStopPanel.SetActive(true);
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
        DisplayNextSentence();
    }

    public void DisplayNextSentence(bool isFirstSentence = false)
    {
        if (sentences.Count == 0)
        {
            StopAllCoroutines();
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