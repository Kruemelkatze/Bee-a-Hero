using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap playAreaTilemap;
    [SerializeField] private Tilemap tileSelectionTilemap;
    
    private TileBase selectedTile;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    
    // Start is called before the first frame update
    void Start()
    {
        selectedTile = tileSelectionTilemap.GetTile(new Vector3Int(6, 2, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int position = grid.WorldToCell(mouseWorldPos);

            if (backgroundTilemap.HasTile(position) == true)
            {
                if (playAreaTilemap.HasTile(position) == false)
                {
                    playAreaTilemap.SetTile(position, selectedTile);
                }
            }

            if (tileSelectionTilemap.HasTile(position) == true)
            {
                selectedTile = tileSelectionTilemap.GetTile(position);
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
