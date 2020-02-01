using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using FTG.AudioController;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TileController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Grid grid;
    [SerializeField] private Tile playAreaTile;
    [SerializeField] private Tilemap backgroundTileMap;
    [SerializeField] private Tilemap playAreaTileMap;
    [SerializeField] private Tilemap tileSelectionTileMap;
    [SerializeField] private GameObject honeycombPrefab;
    [SerializeField] private Transform honeycombContainer;
    [SerializeField] private Transform availableHoneycombContainer;

    [SerializeField] private int creationCounter = 0;
    
    private Honeycomb selectedHoneycomb;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    
    // Start is called before the first frame update
    void Start()
    {
        // randomize selection honeycombs
        GameObject newSelectionHoneycomb = Instantiate(honeycombPrefab, tileSelectionTileMap.CellToWorld(new Vector3Int(6, 2, 0)), Quaternion.identity,
            availableHoneycombContainer);
        newSelectionHoneycomb.GetComponent<Honeycomb>().RandomizeWalls();
        
        newSelectionHoneycomb = Instantiate(honeycombPrefab, tileSelectionTileMap.CellToWorld(new Vector3Int(6, 0, 0)), Quaternion.identity,
            availableHoneycombContainer);
        newSelectionHoneycomb.GetComponent<Honeycomb>().RandomizeWalls();
        
        newSelectionHoneycomb = Instantiate(honeycombPrefab, tileSelectionTileMap.CellToWorld(new Vector3Int(6, -2, 0)), Quaternion.identity,
            availableHoneycombContainer);
        newSelectionHoneycomb.GetComponent<Honeycomb>().RandomizeWalls();

        selectedHoneycomb = null;
        
        Setup();
    }
    
    private void OnGUI()
    {
//        PrintTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameController.Instance.IsPaused == true) ||
            (GameController.Instance.IsRunning == false) ||
            (GameController.Instance.IsGameOver == true))
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0) == true)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int position = grid.WorldToCell(mouseWorldPos);

            // check if a free space on the play area was clicked and if a honeycomb is selected
            if (backgroundTileMap.HasTile(position) == true)
            {
                if (playAreaTileMap.HasTile(position) == false)
                {
                    if (selectedHoneycomb != null)
                    {
                        AudioController.Instance.PlaySound("PlacementSound");
                        
                        creationCounter++;
                        
                        // place the selected honeycomb
                        playAreaTileMap.SetTile(position, playAreaTile);
                        GameObject newHoneycomb = Instantiate(honeycombPrefab, playAreaTileMap.CellToWorld(position),
                            Quaternion.identity, honeycombContainer);
                        var hc = newHoneycomb.GetComponent<Honeycomb>();
                        hc.number = creationCounter;
                        newHoneycomb.name = "HoneyComb " + hc.number;
                        hc.SetWalls(selectedHoneycomb);
                        hc.SetOutsideBounds(backgroundTileMap, position);

                        var honeyCombs = honeycombContainer.GetComponentsInChildren<Honeycomb>();
                        foreach (Honeycomb item in honeyCombs)
                        {
                            var itemHC = item.GetComponent<Honeycomb>();
                            itemHC.FindConnectedHoneycombs(honeyCombs, playAreaTileMap, playAreaTileMap.WorldToCell(item.transform.position));
                            itemHC.SetDoors();
                        }
                        
                        // get new randomized honeycomb and deselect it
                        selectedHoneycomb.RandomizeWalls();
                        selectedHoneycomb.Deselect();
                        selectedHoneycomb = null;
                    }
                }
            }

            // check if a honeycomb from the selection is clicked and set the selected honeycomb
            if (tileSelectionTileMap.HasTile(position) == true)
            {
                string[] grabSounds = 
                {
                    "GrabSound1", 
                    "GrabSound2", 
                    "GrabSound3"
                };

                AudioController.Instance.PlaySound(grabSounds[Random.Range(0, grabSounds.Length)]);
                
                if (selectedHoneycomb != null)
                {
                    selectedHoneycomb.Deselect();
                }

                foreach (Transform item in availableHoneycombContainer)
                {
                    if (item.position == tileSelectionTileMap.CellToWorld(position) == true)
                    {
                        selectedHoneycomb = item.GetComponent<Honeycomb>();
                        selectedHoneycomb.Select();
                    }
                }
            }
        }
    }
    
    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */
    
    // Play Start and Finish Tiles
    private void Setup()
    {
        var startGO = Instantiate(honeycombPrefab, playAreaTileMap.CellToWorld(new Vector3Int(0, -4, 0)),
            Quaternion.identity, honeycombContainer);
        startGO.name = "Start";
        var startHC = startGO.GetComponent<Honeycomb>();
        startHC.SetStartWalls();
        startHC.SetDoors();
        startHC.UpdateWalls();

        var endGO = Instantiate(honeycombPrefab, playAreaTileMap.CellToWorld(new Vector3Int(0, 4, 0)), Quaternion.identity,
            honeycombContainer);
        endGO.name = "End";
        var endHC = endGO.GetComponent<Honeycomb>();
        endHC.SetFinishWalls();
        endHC.SetDoors();
        endHC.UpdateWalls();
    }
    
    private void PrintTiles()
    {
        foreach (var pos in backgroundTileMap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = backgroundTileMap.CellToWorld(localPlace);
            if (backgroundTileMap.HasTile(localPlace))
            {
#if UNITY_EDITOR
                Handles.Label(place + new Vector3(-0.3f,0.125f,0), $"{localPlace}"); // TODO: Remove for Production
#endif
            }
        }
    }
    
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void SetGrid(Grid newGrid)
    {
        grid = newGrid;
    }
    
    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
}
