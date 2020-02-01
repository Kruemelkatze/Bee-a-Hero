using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Grid gridPrefab;
        
    [SerializeField] private Dialogue[] dialogues;
    
    [SerializeField] private Bee bee;
    
    public bool IsPaused { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }
    
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
        var flood = PathfinderBeemaker.GetAllConnectedTiles(bee.currentTile);
        var isAnyOpen = flood.Any(t => t.HasAnyDoor());
        if (!isAnyOpen)
        {
            GameOver();
        }
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
        AudioController.Instance.PlaySound("WinSound");
//        Grid grid = Instantiate(gridPrefab, Vector3.up * 10u, Quaternion.identity, gridContainer);
        
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
        if (targetTile == TileController.Instance.GetFinishHoneycomb())
        {
            Debug.Log("Finished Level");
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

}