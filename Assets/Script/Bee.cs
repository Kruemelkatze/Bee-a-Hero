using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public Honeycomb originTile; // Where Bee was standing
    public Honeycomb currentTile; // Where Bee is standing
    public Honeycomb targetTile; // Where Bee should go

    [SerializeField] bool moving = false;
    [SerializeField] Coroutine activeNavigation;
        
    public float navTimeStep = 0.5f; // [s]
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartTile(Honeycomb startTile)
    {
        transform.position = startTile.transform.position;
        originTile = startTile;
        currentTile = startTile;
    }

    public void Navigate(List<Honeycomb> path)
    {
        if (path.Count == 0) 
            return;

        if (activeNavigation != null)
        {
            StopCoroutine(activeNavigation);
        }

        targetTile = path.Last();
        activeNavigation = StartCoroutine(NavigateAsync(path));
    }

    private IEnumerator NavigateAsync(List<Honeycomb> path)
    {
        moving = true;

        var startIndex = path.First() == originTile ? 1 : 0;

        for (int i = startIndex; i < path.Count; i++)
        {
            var current = path[i];
            yield return new WaitForSeconds(navTimeStep); 
            // TODO: When smoothly navigating and we stop the coroutine, when should we set the currentTile?
            currentTile = current;
            transform.position = current.transform.position;
        }

        originTile = targetTile;
        targetTile = null;
        moving = false;
        GameController.Instance.BeeFinishedNavigating(originTile);
    }
    
    
    [CustomEditor(typeof(Bee))]
    public class BeeEditor : Editor
    {

        public bool IsAnyOpen = true;
        
        public override void OnInspectorGUI()
        {
            var bee = target as Bee;
            DrawDefaultInspector();
            
            if (GUILayout.Button("Check Connected Tiles"))
            {
                var flood = PathfinderBeemaker.GetAllConnectedTiles(bee.currentTile);
                IsAnyOpen = flood.Any(t => t.HasAnyDoor());

                Debug.Log(string.Join(",",flood.Select(x => x.number)));
                Debug.Log(IsAnyOpen);
            }
        }
    }
}
