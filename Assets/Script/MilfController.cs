using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MilfController : MonoBehaviour
{
    public static MilfController Instance;
    [SerializeField] private Transform milfContainer;
    public GameObject milfPrefab;
    public GameObject milfIndicator;

    public List<Milf> spawnedMilfs = new List<Milf>();
    public List<Honeycomb> freeCorrupted = new List<Honeycomb>();
    
    [SerializeField] private int milfCounter = 0;

    
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
    
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        spawnedMilfs.Clear();
        freeCorrupted.Clear();
        
        milfCounter = 0;
        UpdateMilfIndicator();
        PrepareMilfPlacement();
    }

    public void SetLevel(Transform newLevel)
    {
        milfContainer = newLevel.Find("MilfContainer");
        milfIndicator = newLevel.Find("MilfIndicator").gameObject;
    }

    public Honeycomb PrepareMilfPlacement()
    {
        var tile = GetCorruptedTile();
        // Well, do we need anything else?
        return tile;
    }

    public Milf PlaceMilf()
    {
        var tile = GetCorruptedTile();

        if (tile == null)
        {
            return null;
        }
        
        freeCorrupted.Remove(tile);
        var corruptedScript = tile.GetComponent<Corrupted>();
        corruptedScript.MilfPlaced();

        var milfGo = Instantiate(milfPrefab, milfContainer);
        var milf = milfGo.GetComponent<Milf>();
        milf.transform.position = tile.transform.position;
        milf.originTile = tile;
        milf.currentTile = tile;
        spawnedMilfs.Add(milf);
        return milf;
    }
    
    public Honeycomb GetCorruptedTile(bool createNewIfNoneExists = true)
    {
        if (freeCorrupted.Count > 0)
        {
            return freeCorrupted.First();
        }
        
        if (!createNewIfNoneExists)
        {
            return null;
        }
        
        var n = TileController.Instance.SpawnCorrupted();
        freeCorrupted.Add(n);
        return n;
    }

    public int IncreaseMilfCounter()
    {
        milfCounter = (milfCounter + 1) % 7;
        UpdateMilfIndicator();

        if (milfCounter == 6)
        {
            PlaceMilf();
        } else if (milfCounter == 0)
        {
            PrepareMilfPlacement();
        }

        return milfCounter;
    }

    private void UpdateMilfIndicator()
    {
        var walls = milfIndicator.transform.Find("Walls");
        for (int i = 0; i < walls.childCount; i++)
        {
            walls.GetChild(i).gameObject.SetActive(milfCounter > i);
        }
    }

    public void MoveMilfs(Honeycomb beeCurrentTile)
    {
        Debug.Log($"Moving {spawnedMilfs.Count} milfs");
        // Bee segment
        foreach (var milf in spawnedMilfs)
        {
           var path = PathfinderBeemaker.FindPath(milf.currentTile, beeCurrentTile);
           milf.Navigate(path);
        }
    }
    
#if UNITY_EDITOR    
    [CustomEditor(typeof(MilfController))]
    public class MilfControllerEditor : Editor
    {

        public bool IsAnyOpen = true;
        
        public override void OnInspectorGUI()
        {
            var milfController = target as MilfController;
            DrawDefaultInspector();
            
            if (GUILayout.Button("MilfCounter ++"))
            {
                milfController.IncreaseMilfCounter();
            }
            
            if (GUILayout.Button("Preview Milf"))
            {
                milfController.PrepareMilfPlacement();
            }
            
            if (GUILayout.Button("Spawn Milf"))
            {
                milfController.PlaceMilf();
            }
        }
    }
#endif    
}
