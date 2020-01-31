using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenShake : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Camera camera;
    [SerializeField] private float defaultDuration = 0.1f;
    [SerializeField] private float defaultFrequency = 100f;
    [SerializeField] private float defaultIntensity = 0.1f;

    private Vector3 startPosition;
    private float duration = 0.2f;
    private float frequency = 100f;
    private float intensity = 0.1f;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        startPosition = camera.transform.position;
        duration = defaultDuration;
        frequency = defaultFrequency;
        intensity = defaultIntensity;
    }

    private void Update()
    {

    }  
    
    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private IEnumerator ScreenShakeCoroutine()
    {
        for (int i = 0; i < (int)(duration * frequency); i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 random_offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), -10f) * intensity;
            camera.transform.position = startPosition + random_offset;
            yield return new WaitForSecondsRealtime(1f / frequency);
        }
        camera.transform.position = startPosition;
        duration = defaultDuration;
        frequency = defaultFrequency;
        intensity = defaultIntensity;
    }

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void ShakeScreen(float newDuration = -1f, float newFrequency = -1f, float newIntensity = -1f)
    {
        float maxTolerance = 0.001f;
        if (Math.Abs(newDuration - (-1f)) > maxTolerance)
        {
            duration = newDuration;
        }

        if (Math.Abs(newFrequency - (-1f)) > maxTolerance)
        {
            frequency = newFrequency;
        }

        if (Math.Abs(newIntensity - (-1f)) > maxTolerance)
        {
            intensity = newIntensity;
        }
        
        transform.position = startPosition;
        StopCoroutine(ScreenShakeCoroutine());
        StartCoroutine(ScreenShakeCoroutine());
    }
    
    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}