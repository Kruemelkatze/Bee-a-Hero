using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FTG.AudioController;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static GameController Instance;
    
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject gameOverOverlay;
    [SerializeField] private Transform levelPrefab;
    [SerializeField] private Transform levitatingObjects;
    [SerializeField] private TileController tileController;
    [SerializeField] private float transitionTime = 3f;
    [SerializeField] private Dialogue[] dialogues;
    [SerializeField] private Bee bee;
    
    public bool IsPaused { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsFinished { get; private set; }
    
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
        IsRunning = false;
        IsGameOver = false;
        IsFinished = false;
        
        pauseOverlay.SetActive(false);
        gameOverOverlay.SetActive(false);
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

    private IEnumerator LevelFinishedCoroutine()
    {
        AudioController.Instance.TransitionToSnapshot("SilentSnapshot", 0.5f);
        yield return new WaitForSeconds(0.5f);
        AudioController.Instance.PlaySound("WinSound");
        yield return new WaitForSeconds(5f);
        AudioController.Instance.TransitionToSnapshot("GamePlaySnapshot", 0.5f);

        Transform oldLevel = tileController.GetLevel();
        Transform newLevel = Instantiate(levelPrefab, Vector3.up * 10u, Quaternion.identity, levitatingObjects);

        tileController.SetLevel(newLevel);
        tileController.Setup();
        
        oldLevel.DOMoveY(-10f, transitionTime);
        newLevel.DOMoveY(0f, transitionTime);

        yield return new WaitForSeconds(transitionTime);

        bee = newLevel.transform.Find("Bee").GetComponent<Bee>();
        Destroy(oldLevel.gameObject);

        IsFinished = false;
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
            IsRunning = true;
            return null;
        }
    }

    public void GameOver()
    {
        if (IsGameOver == false)
        {
            IsGameOver = true;
            gameOverOverlay.SetActive(true);

            AudioController.Instance.PlaySound("GameOverSound");
            AudioController.Instance.StopAllMusic();
        }
    }

    public void LevelFinished()
    {
        if (IsFinished == true)
        {
            return;
        }
        
        IsFinished = true;
        StartCoroutine(LevelFinishedCoroutine());
    }

    public void ActivateMilf()
    {
        AudioController.Instance.PlayMusic("MilfMusic");
        AudioController.Instance.TransitionToSnapshot("GamePlayMilfSnapshot");
    }

    public void StartTilePlaced(Honeycomb tile)
    {
        bee.SetStartTile(tile);
    }

    public void TilePlaced(Honeycomb tile)
    {
        var path = PathfinderBeemaker.FindPath(bee.currentTile, TileController.Instance.GetFinishHoneycomb());
        if (path.Count == 0)
        {
            // No Path to finish found, get to target tile
            path = PathfinderBeemaker.FindPath(bee.currentTile, tile);
        }
        bee.Navigate(path);
    }

    public void BeeFinishedNavigating(Honeycomb targetTile)
    {
        if (IsFinished == true)
        {
            return;
        }
        
        if (targetTile == TileController.Instance.GetFinishHoneycomb())
        {
            LevelFinished();
        }
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}