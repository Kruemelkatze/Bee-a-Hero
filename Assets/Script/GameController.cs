using System;
using System.Collections;
using System.Collections.Generic;
using FTG.AudioController;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static GameController Instance;
    
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private Dialogue[] dialogues;
    
    public bool IsPaused { get; private set; }
    public bool isRunning { get; private set; }
    private int currentDialogue;
    
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
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        IsPaused = false;
        pauseOverlay.SetActive(false);
        currentDialogue = 0;

        AudioController.Instance.PlayMusic("GamePlayMusic", false);
        AudioController.Instance.TransitionToSnapshot("GamePlaySnapshot", 0.5f);
        AudioController.Instance.StopMusic("MilfMusic", 1f);
        AudioController.Instance.StopMusic("MenuMusic", 1f);

        StartCoroutine(StartIntroDialogue());
    }

    private void Update()
    {
        
    }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private IEnumerator StartIntroDialogue()
    {
        yield return new WaitForSeconds(1f);
        DialogueController.Instance.StartDialogue(dialogues[currentDialogue]);
    }
    
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void PauseGame()
    {
        IsPaused = true;
        pauseOverlay.SetActive(true);
    }

    public void ContinueGame()
    {
        IsPaused = false;
        pauseOverlay.SetActive(false);
    }

    public Dialogue GetNextDialogue()
    {
        currentDialogue++;
        if (currentDialogue < dialogues.Length)
        {
            return dialogues[currentDialogue];
        }
        else
        {
            isRunning = true;
            return null;
        }
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}