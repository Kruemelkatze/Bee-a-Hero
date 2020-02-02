using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static TileController Instance;
    
    [SerializeField] private Grid grid;
    [SerializeField] private Tile playAreaTile;
    [SerializeField] private Tilemap backgroundTileMap;
    [SerializeField] private Tilemap playAreaTileMap;
    [SerializeField] private Tilemap tileSelectionTileMap;
    [SerializeField] private GameObject honeycombPrefab;
    [SerializeField] private GameObject corruptedPrefab;
    [SerializeField] private Transform honeycombContainer;
    [SerializeField] private Transform availableHoneycombContainer;
    [SerializeField] private Transform level;
    [SerializeField] private Transform placementEffectPrefab;
    
    [SerializeField] private int creationCounter = 0;
    [SerializeField] private int corruptedCounter = 0;

    private Honeycomb selectedHoneycomb;
    private Honeycomb startHoneycomb;
    private Honeycomb finishHoneycomb;

    [Header("Spawn Percentages")] 
    [Range(0, 1f)]
    public float p0 = 0;
    [Range(0, 1f)]
    public float p1 = 0.15f;
    [Range(0, 1f)]
    public float p2 = 0.35f;
    [Range(0, 1f)]
    public float p3 = 0.25f;
    [Range(0, 1f)]
    public float p4 = 0.1f;
    [Range(0, 1f)]
    public float p5 = 0.1f;
    [Range(0, 1f)]
    public float p6 = 0.05f;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }
    
    private void OnGUI()
    {
        //PrintTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameController.Instance.IsPaused == true) ||
            (GameController.Instance.IsRunning == false) ||
            (GameController.Instance.IsGameOver == true) ||
            (GameController.Instance.IsFinished == true))
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
                        var hc = PlaceTile(position, false);
                        
                        // get new randomized honeycomb and deselect it
                        selectedHoneycomb.InitWalls(GetRandomEdgeDefinition());
                        selectedHoneycomb.Deselect();
                        selectedHoneycomb = null;
                        
                        GameController.Instance.TilePlaced(hc, false);
                        ScoreController.Instance.IncrementTilesUsed();
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

    private Honeycomb PlaceTile(Vector3Int position, bool isCorrupted)
    {
        GameObject prefab;
        if (isCorrupted)
        {
            corruptedCounter++;
            prefab = corruptedPrefab;
        }
        else
        {
            creationCounter++;
            prefab = honeycombPrefab;
        }
        
        AudioController.Instance.PlaySound("PlacementSound");

        // place the selected honeycomb
        playAreaTileMap.SetTile(position, playAreaTile);
        
        GameObject newHoneycomb = Instantiate(prefab, playAreaTileMap.CellToWorld(position), Quaternion.identity, honeycombContainer);
        
        var hc = newHoneycomb.GetComponent<Honeycomb>();
        if (isCorrupted)
        {
            hc.number = -corruptedCounter;
            newHoneycomb.name = "Corrupted " + corruptedCounter;
            // Walls are defined by prefab
        }
        else
        {
            hc.number = creationCounter;
            newHoneycomb.name = "HoneyComb " + hc.number; 
            hc.SetWalls(selectedHoneycomb);
        }

        hc.SetOutsideBounds(backgroundTileMap, position);
                        
        Instantiate(placementEffectPrefab, playAreaTileMap.CellToWorld(position) - Vector3.up * 0.17f,
            Quaternion.identity, honeycombContainer);

        var honeyCombs = honeycombContainer.GetComponentsInChildren<Honeycomb>();
        foreach (Honeycomb item in honeyCombs)
        {
            var itemHC = item.GetComponent<Honeycomb>();
            itemHC.FindConnectedHoneycombs(honeyCombs, playAreaTileMap, playAreaTileMap.WorldToCell(item.transform.position));
            itemHC.SetDoors();
        }

        return hc;
    }

    private void OnValidate()
    {
        var pSum = p0 + p1 + p2 + p3 + p4 + p5 + p6;
        if (pSum == 0)
        {
            p0 = 0;
            p1 = 1/6f;
            p2 = 1/6f;
            p3 = 1/6f;
            p4 = 1/6f;
            p5 = 1/6f;
            p6 = 1/6f;
        }
        else
        {
            p0 /= pSum;
            p1 /= pSum;
            p2 /= pSum;
            p3 /= pSum;
            p4 /= pSum;
            p5 /= pSum;
            p6 /= pSum;
        }
    }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void PrintTiles()
    {
        foreach (var pos in tileSelectionTileMap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tileSelectionTileMap.CellToWorld(localPlace);
            if (tileSelectionTileMap.HasTile(localPlace))
            {
#if UNITY_EDITOR
                Handles.Label(place + new Vector3(-0.3f,0.125f,0), $"{localPlace}"); // TODO: Remove for Production
#endif
            }
        }
    }

    private List<Vector3Int> GetFreeTilePositions()
    {
        var free = new List<Vector3Int>();
        foreach (var pos in backgroundTileMap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (backgroundTileMap.HasTile(localPlace) && !playAreaTileMap.HasTile(localPlace) )
            {
                free.Add(localPlace);
            }
        }

        return free;
    }

    private int GetWeightedRandomEntry(float[] percentages)
    {
        var sum = percentages.Sum();
        var percentageSums = new float[percentages.Length];
        percentageSums[0] = percentages[0] / sum;
        
        for (int i = 1; i < percentages.Length; i++)
        {
            percentageSums[i] = percentages[i] / sum + percentageSums[i - 1];
        }

        var random = Random.Range(0f, 1f);

        var chosen = 0;
        for (int i = 1; i < percentageSums.Length; i++)
        {
            if (percentageSums[i] > random)
            {
                return i;
            }

            chosen = i;
        }

        return chosen;
    }
    
    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    // Play Start and Finish Tiles
    public void Setup()
    {
        // randomize selection honeycombs
        GameObject newSelectionHoneycomb = Instantiate(honeycombPrefab, tileSelectionTileMap.CellToWorld(new Vector3Int(6, 2, 0)), Quaternion.identity,
            availableHoneycombContainer);
        newSelectionHoneycomb.GetComponent<Honeycomb>().InitWalls(GetRandomEdgeDefinition());
        
        newSelectionHoneycomb = Instantiate(honeycombPrefab, tileSelectionTileMap.CellToWorld(new Vector3Int(6, 0, 0)), Quaternion.identity,
            availableHoneycombContainer);
        newSelectionHoneycomb.GetComponent<Honeycomb>().InitWalls(GetRandomEdgeDefinition());
        
        newSelectionHoneycomb = Instantiate(honeycombPrefab, tileSelectionTileMap.CellToWorld(new Vector3Int(6, -2, 0)), Quaternion.identity,
            availableHoneycombContainer);
        newSelectionHoneycomb.GetComponent<Honeycomb>().InitWalls(GetRandomEdgeDefinition());

        selectedHoneycomb = null;
        
        var startGO = Instantiate(honeycombPrefab, playAreaTileMap.CellToWorld(new Vector3Int(0, -4, 0)), Quaternion.identity, honeycombContainer);
        startGO.name = "Start";
        startHoneycomb = startGO.GetComponent<Honeycomb>();
        startHoneycomb.SetStartWalls();
        startHoneycomb.SetDoors();
        startHoneycomb.UpdateWalls();
        
        GameController.Instance.StartTilePlaced(startHoneycomb);
        
        var finishGO = Instantiate(honeycombPrefab, playAreaTileMap.CellToWorld(new Vector3Int(0, 4, 0)), Quaternion.identity, honeycombContainer);
        finishGO.name = "End";
        finishHoneycomb = finishGO.GetComponent<Honeycomb>();
        finishHoneycomb.SetFinishWalls();
        finishHoneycomb.SetDoors();
        finishHoneycomb.UpdateWalls();
        finishGO.transform.Find("Honeycomb").GetComponent<SpriteRenderer>().sortingOrder = 10;
    }
    
    public void SetLevel(Transform newLevel)
    {
        level = newLevel;

        grid = level.Find("Grid").GetComponent<Grid>();
        backgroundTileMap = level.Find("Grid").Find("Background").GetComponent<Tilemap>();
        playAreaTileMap = level.Find("Grid").Find("PlayArea").GetComponent<Tilemap>();
        tileSelectionTileMap = level.Find("Grid").Find("TileSelection").GetComponent<Tilemap>();
        honeycombContainer = level.Find("HoneycombContainer");
        availableHoneycombContainer = level.Find("AvailableHoneycombContainer");
    }

    private List<TileBase> GetAllTiles(Tilemap tilemap)
    {
        var tiles = new List<TileBase>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            //Vector3 place = backgroundTileMap.CellToWorld(localPlace);
            var tile = tilemap.GetTile(localPlace);
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }

        return tiles;
    }
    
    public Transform GetLevel()
    {
        return level;
    }
    
    public Honeycomb GetFinishHoneycomb()
    {
        return finishHoneycomb;
    }

    public bool IsSpaceLeft()
    {
        var tileCount = GetAllTiles(backgroundTileMap).Count;
        return honeycombContainer.childCount < tileCount;
    }

    public int GetWeightedRandomOpeningCount()
    {
        var percentages = new []{ p0,p1,p2,p3,p4,p5,p6 };
        var entry = GetWeightedRandomEntry(percentages);
        return entry;
    }

    private static int[] Edges = {0, 1, 2, 3, 4, 5};
    public IEnumerable<int> GetRandomEdges(int count)
    {
        return Edges.OrderBy(x => Random.value).Take(count);
    }
    
    public bool[] GetRandomEdgeDefinition()
    {
        var def = new bool[6];
        var edges = Edges.OrderBy(x => Random.value).Take(GetWeightedRandomOpeningCount());
        foreach (var edge in edges)
        {
            def[edge] = true;
        }

        return def;
    }

    public Honeycomb SpawnCorrupted(Vector3Int? pos = null)
    {
        pos = pos ?? GetFreeTilePosition();

        var c = PlaceTile(pos.Value, true);
        return c;
    }

    public Vector3Int GetFreeTilePosition()
    {
        var free = GetFreeTilePositions();
        var pos = Random.Range(0, free.Count - 1);
        return free[pos];
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
    
#if UNITY_EDITOR
    [CustomEditor(typeof(TileController))]
    public class TileControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var tileController = target as TileController;
            DrawDefaultInspector();

            if (GUILayout.Button("Get Weighted Entry"))
            {
                var count = tileController.GetWeightedRandomOpeningCount();
                var edges = tileController.GetRandomEdges(count);
                var def = tileController.GetRandomEdgeDefinition();
                Debug.Log(string.Join(",", def));
            }
        }
    }
#endif    
}
