using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public Honeycomb originTile; // Where Bee was standing
    public Honeycomb currentTile; // Where Bee is standing
    public Honeycomb targetTile; // Where Bee should go

    [SerializeField] bool moving = false;
        
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
        
        targetTile = path.Last();
        StartCoroutine(NavigateAsync(path));
    }

    private IEnumerator NavigateAsync(List<Honeycomb> path)
    {
        moving = true;

        var startIndex = path.First() == originTile ? 1 : 0;

        for (int i = startIndex; i < path.Count; i++)
        {
            var current = path[i];
            yield return new WaitForSeconds(navTimeStep);
            currentTile = current;
            transform.position = current.transform.position;
        }

        originTile = targetTile;
        targetTile = null;
        moving = false;
        GameController.Instance.BeeFinishedNavigating(originTile);
    }
}
