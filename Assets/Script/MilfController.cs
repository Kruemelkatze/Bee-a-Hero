using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilfController : MonoBehaviour
{
    public static MilfController Instance;
    public GameObject MilfPrefab;
    public List<Milf> SpawnedMilfs = new List<Milf>();
    
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
                
            }
            
            if (GUILayout.Button("Spawn Milf"))
            {
                
            }
        }
    }
}
