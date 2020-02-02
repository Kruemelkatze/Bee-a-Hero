using System.Collections;
using System.Collections.Generic;
using FTG.AudioController;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject exitButton;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        settingsPanel.SetActive(false);

        AudioController.Instance.PlayMusic("MenuMusic", false);
        AudioController.Instance.TransitionToSnapshot("MenuSnapshot", 0.5f);
        AudioController.Instance.StopMusic("GamePlayMusic", 1f);
        AudioController.Instance.StopMusic("MilfMusic", 1f);
        
#if UNITY_WEBGL
        exitButton.SetActive(false);
#else
        exitButton.SetActive(true);        
#endif
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

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}