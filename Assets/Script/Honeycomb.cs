using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Honeycomb : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Transform wallSprites;
    
    private bool[] walls;
    private Honeycomb[] connectedHoneycombs;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    
    private void Start()
    {

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

    public void RandomizeWalls()
    {
        walls = new bool[6];
        
        for (int i = 0; i < walls.Length; i++)
        {
            bool hasWall = Random.Range(0f, 1f) > 0.5f;
            walls[i] = hasWall;
            wallSprites.GetChild(i).gameObject.SetActive(hasWall);
        }
    }
    
    public void SetWalls(Honeycomb honeycomb)
    {
        walls = new bool[6];
        walls = honeycomb.GetWalls();
        
        for (int i = 0; i < walls.Length; i++)
        {
            wallSprites.GetChild(i).gameObject.SetActive(walls[i]);
        }
    }

    public bool[] GetWalls()
    {
        return walls;
    }
    
    /// <summary>
    /// Checking the tiles in counterclockwise order, starting with the tile on the right side and writing the tiles to the connectedTiles array
    /// </summary>
    /// <param name="HoneycompContainer"></param>
    /// <param name="playAreaTileMap"></param>
    /// <param name="position"></param>
    public void GetConnectedHoneycombs(Transform HoneycompContainer, Tilemap playAreaTileMap, Vector3Int position)
    {
        connectedHoneycombs = new Honeycomb[6];
        
        Vector3Int[] offsets = new[]
        {
            new Vector3Int(1, 0, 0), // right
            new Vector3Int(0, 1, 0), // top right
            new Vector3Int(-1, 1, 0), // top left
            new Vector3Int(-1, 0, 0), // left
            new Vector3Int(-1, -1, 0), // bottom left
            new Vector3Int(0, -1, 0) // bottom right
        };

        for (int i = 0; i < connectedHoneycombs.Length; i++)
        {
            Vector3Int checkedPosition = position + offsets[i];
            Vector3 worldPosition = playAreaTileMap.CellToWorld(checkedPosition);
            foreach (Transform item in HoneycompContainer)
            {
                if (item.position == worldPosition)
                {
                    connectedHoneycombs[i] = item.GetComponent<Honeycomb>();
                }
            }
        }
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}