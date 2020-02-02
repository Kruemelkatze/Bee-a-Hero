using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OtterController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Transform[] SpawnPositions;
    [SerializeField] private float maxSpawnRadius;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private Transform otterPrefab;
    [SerializeField] private float minRotationSpeed;
    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float movementSpeed;
    
    
    private float spawnTimer;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
            SpawnOtter();
        }
    }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void SpawnOtter()
    {
        int startIndex = Random.Range(0, SpawnPositions.Length);
        int endIndex = (startIndex + 2) % SpawnPositions.Length;
        
        float randomAngle = Random.Range(0f, 2 * Mathf.PI);
        float randomRadius = Random.Range(0f, maxSpawnRadius);
        Vector3 spawnPosition = SpawnPositions[startIndex].position +
                                randomRadius * new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
        
        randomAngle = Random.Range(0f, 2 * Mathf.PI);
        randomRadius = Random.Range(0f, maxSpawnRadius);
        Vector3 endPosition = SpawnPositions[endIndex].position +
                              randomRadius * new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);

        randomAngle = Random.Range(0f, 360f);
        Transform otter = Instantiate(otterPrefab, spawnPosition, Quaternion.Euler(0f, 0f, randomAngle), null);
        float travelTime = Vector3.Distance(spawnPosition, endPosition) / movementSpeed;
        otter.DOMove(endPosition, travelTime);
        float randomRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        randomRotationSpeed = Random.Range(0f, 1f) > 0.5 ? randomRotationSpeed : -randomRotationSpeed;
        otter.DORotate(travelTime * randomRotationSpeed * Vector3.forward, travelTime);
        Destroy(otter.gameObject, travelTime);
    }
    
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}