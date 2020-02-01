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
    [SerializeField] private Transform doorSprites;
    
    public int number;
    
    private bool[] walls = new bool[6];
    private bool[] doors = new bool[6];
    private bool[] blocked = new bool[6];
    private Honeycomb[] connectedHoneycombs = new Honeycomb[6];
    
    private Vector3Int[] yEvenOffsets = new[]
    {
        new Vector3Int(1, 0, 0),     // right
        new Vector3Int(0, 1, 0),     // top right
        new Vector3Int(-1,1,0),    // top left
        new Vector3Int(-1, 0, 0),    // left
        new Vector3Int(-1, -1, 0),   // bottom left
        new Vector3Int(0, -1, 0) // bottom right
    };
    
    private Vector3Int[] yUnevenOffsets = new[]
    {
        new Vector3Int(1, 0, 0),     // right
        new Vector3Int(1,1,0),     // top right
        new Vector3Int(0, 1, 0),    // top left
        new Vector3Int(-1, 0, 0),    // left
        new Vector3Int(0, -1, 0),   // bottom left
        new Vector3Int(1, -1, 0) // bottom right
    };
    
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
    public bool IsSideOpen(int side)
    {
        // R, TR, TL, L, BL, BR
        return !walls[side] && !blocked[side];
    }

    public bool IsOppositeSideOpen(int side) => IsSideOpen((side + 3) % 6);
    
    public Honeycomb[] GetConnectedHoneycombs()
    {
        return connectedHoneycombs;
    }
    
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

    public void SetStartWalls()
    {
        walls = new [] { true, false, false, true, true, true };
    }

    public void SetFinishWalls()
    {
        walls = new [] { true, true, true, true, false, false };
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

    public void SetDoors()
    {
        // Walls and connected tiles are already set
        // Set doors to position where no wall is present and no connected tile is set
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i] = IsSideOpen(i) && !connectedHoneycombs[i];
            doorSprites.GetChild(i).gameObject.SetActive(doors[i]);
        }
        
        UpdateWalls();
    }

    public void UpdateWalls()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            walls[i] = !doors[i] && !IsSideOpen(i);
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
    public void FindConnectedHoneycombs(Honeycomb[] honeyCombs, Tilemap playAreaTileMap, Vector3Int position)
    {
        // Walls are already set
        connectedHoneycombs = new Honeycomb[6];

        var yEven = position.y % 2 == 0;
        // set offsets for counterclockwise checking of the honeycombs
        Vector3Int[] offsets = yEven ? yEvenOffsets : yUnevenOffsets;

        // loop over the counterclockwise positions and set the connectedHoneycombs
        for (int i = 0; i < connectedHoneycombs.Length; i++)
        {
            if (!IsSideOpen(i))
            {
                continue;
            }
            Vector3Int checkedPosition = position + offsets[i];
            Vector3 worldPosition = playAreaTileMap.CellToWorld(checkedPosition);
            foreach (Honeycomb item in honeyCombs)
            {
                if ( Vector3.Distance(item.transform.position,worldPosition) < 0.1f)
                {
                    var oppositeSideOpen = item.IsOppositeSideOpen(i);
                    blocked[i] = !oppositeSideOpen;
                    if (oppositeSideOpen)
                    {
                        connectedHoneycombs[i] = item;
                    }
                }
            }
        }
    }

    public void SetOutsideBounds(Tilemap bgTileMap, Vector3Int position)
    {
        var yEven = position.y % 2 == 0;
        // set offsets for counterclockwise checking of the honeycombs
        Vector3Int[] offsets = yEven ? yEvenOffsets : yUnevenOffsets;
        for (int i = 0; i < offsets.Length; i++)
        {
            blocked[i] = bgTileMap.GetTile(position + offsets[i]) == null;
        }
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}