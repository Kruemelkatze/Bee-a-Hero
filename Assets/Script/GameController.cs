using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
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

    private void CheckNoOptionLeft()
    {
        //Check if target segment is reachable
        var finishSegment = PathfinderBeemaker.GetAllConnectedTiles(TileController.Instance.GetFinishHoneycomb());
        var beeIsInFinishSegment = finishSegment.Contains(bee.currentTile); // Bee is moving
        if (!beeIsInFinishSegment)
        {
            var isAnyOpenInFinishSegment = finishSegment.Any(t => t.HasAnyDoor());
            if (!isAnyOpenInFinishSegment)
            {
                StartCoroutine(
                    GameOverWithAnItsyBitsyTeenyWeenyNoImmediateDeathMachiney(GameOverReason.TargetPathBlocked));
            }
        }
        
        // Check if there if the segment the bee is on has at least one open connection
        var flood = PathfinderBeemaker.GetAllConnectedTiles(bee.currentTile);
        var isAnyOpen = flood.Any(t => t.HasAnyDoor());
        if (!isAnyOpen)
        {
            StartCoroutine(
                GameOverWithAnItsyBitsyTeenyWeenyNoImmediateDeathMachiney(GameOverReason.BeePathBlocked));
        }
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

        bee = newLevel.transform.Find("Bee").GetComponent<Bee>();

        tileController.SetLevel(newLevel);
        tileController.Setup();
        
        oldLevel.DOMoveY(-10f, transitionTime);
        newLevel.DOMoveY(0f, transitionTime);

        yield return new WaitForSeconds(transitionTime);

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

    public IEnumerator GameOverWithAnItsyBitsyTeenyWeenyNoImmediateDeathMachiney(
        GameOverReason reason = GameOverReason.Undefined)
    {
        IsGameOver = true;
        AudioController.Instance.PlaySound("GameOverSound");
        AudioController.Instance.StopAllMusic();
        
        yield return new WaitForSeconds(1);
        GameOver(reason);
    }

    public void GameOver(GameOverReason reason = GameOverReason.Undefined)
    {
        if (!IsGameOver)
        {
            AudioController.Instance.PlaySound("GameOverSound");
            AudioController.Instance.StopAllMusic();
        }

        Debug.Log("GameOver Reason: " + Enum.GetName(typeof(GameOverReason), reason));
        
        IsGameOver = true; 
        gameOverOverlay.SetActive(true);
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

        // No path to target found. Be will not move. Check losing condition
        if (path.Count == 0)
        {
            CheckNoOptionLeft();
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
        else
        {
            // Be has moved to new tile. Check loosing condition.
            CheckNoOptionLeft();
        }
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

    public enum GameOverReason
    {
        BeePathBlocked,
        TargetPathBlocked,
        Undefined,
    }
}
