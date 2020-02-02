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

    public List<Milf> spawnedMilfs = new List<Milf>();
    public List<Honeycomb> freeCorrupted = new List<Honeycomb>();
    
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
    
    [CustomEditor(typeof(MilfController))]
    public class MilfControllerEditor : Editor
    {

        public bool IsAnyOpen = true;
        
        public override void OnInspectorGUI()
        {
            var milfController = target as MilfController;
            DrawDefaultInspector();
            
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
}
