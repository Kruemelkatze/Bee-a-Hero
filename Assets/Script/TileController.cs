﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    
    private Honeycomb selectedHoneycomb;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    
    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int position = grid.WorldToCell(mouseWorldPos);

            if (backgroundTileMap.HasTile(position) == true)
            {
                if (playAreaTileMap.HasTile(position) == false)
                {
                    if (selectedHoneycomb != null)
                    {
                        playAreaTileMap.SetTile(position, playAreaTile);
                        GameObject newHoneycomb = Instantiate(honeycombPrefab, playAreaTileMap.CellToWorld(position),
                            Quaternion.identity, honeycombContainer);
                        newHoneycomb.GetComponent<Honeycomb>().SetWalls(selectedHoneycomb);
                        foreach (Transform item in honeycombContainer)
                        {
                            item.GetComponent<Honeycomb>().GetConnectedHoneycombs(honeycombContainer, playAreaTileMap, playAreaTileMap.WorldToCell(item.position));
                        }
                    }
                }
            }

            if (tileSelectionTileMap.HasTile(position) == true)
            {
                foreach (Transform item in availableHoneycombContainer)
                {
                    if (item.position == tileSelectionTileMap.CellToWorld(position) == true)
                    {
                        selectedHoneycomb = item.GetComponent<Honeycomb>();
                    }
                }
            }
        }
    }
    
    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

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
