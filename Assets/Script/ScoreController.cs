using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    static public ScoreController Instance;
    
    [SerializeField] private TextMeshProUGUI levelsFinishedText;
    [SerializeField] private TextMeshProUGUI tilesUsedText;

    private int levelsFinished;
    private int tilesUsed;
    
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
        levelsFinished = 0;
        tilesUsed = 0;
    }

    private void Update()
    {
        
    }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void IncrementLevelsFinished()
    {
        levelsFinished++;
    }

    public void IncrementTilesUsed()
    {
        tilesUsed++;
    }

    public void UpdateGameOverPanel()
    {
        levelsFinishedText.text = levelsFinished.ToString();
        tilesUsedText.text = tilesUsed.ToString();
    }

    public int GetLevelsFinished()
    {
        return levelsFinished;
    }
    
    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}