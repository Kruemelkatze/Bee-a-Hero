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
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject settingsButtonReplacement;
    [SerializeField] private GameObject creditsButton;
    [SerializeField] private GameObject creditsButtonReplacement;
    
    [SerializeField] private GameObject exitButton;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        ClosePanels();
        
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
        ClosePanels();
        
        settingsPanel.SetActive(true);
        settingsButton.SetActive(false);
        settingsButtonReplacement.SetActive(true);
    }

    public void OpenCredits()
    {
        ClosePanels();
        
        creditsPanel.SetActive(true);
        creditsButton.SetActive(false);
        creditsButtonReplacement.SetActive(true);;
    }

    public void ClosePanels()
    {
        settingsPanel.SetActive(false);
        settingsButton.SetActive(true);
        settingsButtonReplacement.SetActive(false);
        settingsButton.transform.localPosition = Vector3.zero;

        creditsPanel.SetActive(false);
        creditsButton.SetActive(true);
        creditsButtonReplacement.SetActive(false);
        creditsButton.transform.localPosition = Vector3.zero;
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}