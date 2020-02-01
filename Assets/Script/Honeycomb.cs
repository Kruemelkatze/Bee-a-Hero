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

    [SerializeField] private GameObject selectionHighlight;
    [SerializeField] private Transform wallSprites;
    
    private bool[] walls;
    private Honeycomb[] connectedHoneycombs;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    
    private void Start()
    {
        selectionHighlight.SetActive(false);
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
    
    // Set the walls based on another honeycomb (selection)
    public void SetWalls(Honeycomb honeycomb)
    {
        walls = new bool[6];
        walls = honeycomb.GetWalls();
        
        for (int i = 0; i < walls.Length; i++)
        {
            wallSprites.GetChild(i).gameObject.SetActive(walls[i]);
        }
    }

    // Get walls configuration
    public bool[] GetWalls()
    {
        return walls;
    }
    
    public void Select()
    {
        selectionHighlight.SetActive(true);
    }

    public void Deselect()
    {
        selectionHighlight.SetActive(false);
    }
    
    // Checking the honeycomb in counterclockwise order, starting with the honeycomb on the right side and writing the honeycomb to the connectedHoneycombs array
    public void GetConnectedHoneycombs(Transform HoneycompContainer, Tilemap playAreaTileMap, Vector3Int position)
    {
        connectedHoneycombs = new Honeycomb[6];
        
        // set offsets for counterclockwise checking of the honeycombs
        Vector3Int[] offsets = new[]
        {
            new Vector3Int(1, 0, 0),     // right
            new Vector3Int(0, 1, 0),     // top right
            new Vector3Int(-1, 1, 0),    // top left
            new Vector3Int(-1, 0, 0),    // left
            new Vector3Int(-1, -1, 0),   // bottom left
            new Vector3Int(0, -1, 0)     // bottom right
        };

        // loop over the counterclockwise positions and set the connectedHoneycombs
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