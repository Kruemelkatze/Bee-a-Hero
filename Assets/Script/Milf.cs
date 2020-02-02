using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FTG.AudioController;
using UnityEngine;

public class Milf : MonoBehaviour
{
    public Honeycomb originTile; // Where Milf was standing
    public Honeycomb currentTile; // Where Milf is standing
    public Honeycomb targetTile; // Where Milf should go
    
    [SerializeField] Transform levitatingObjects;
    [SerializeField] Coroutine activeNavigation;
    [SerializeField] bool moving;
    
    [SerializeField] float movementSpeed = 1f;

    private void Start()
    {
        levitatingObjects = transform.parent.parent.parent; // Oh noes
    }

    public void Navigate(List<Honeycomb> path)
    {
        if (path.Count == 0) 
            return;

        if (activeNavigation != null)
        {
            StopCoroutine(activeNavigation);
        }

        targetTile = path.FirstOrDefault(x => x != currentTile);
        if (targetTile == null) 
            return;
        
        var singlePath = new List<Honeycomb>(1);
        singlePath.Add(targetTile);
        
        activeNavigation = StartCoroutine(NavigateAsync(singlePath));
    }
    
    private IEnumerator NavigateAsync(List<Honeycomb> path)
    {
        moving = true;

        var startIndex = path.First() == originTile ? 1 : 0;

        for (int i = startIndex; i < path.Count; i++)
        {
            var current = path[i];
            transform.DOLocalMove(current.transform.position - levitatingObjects.position, 1f / movementSpeed);
            yield return new WaitForSeconds(1f / movementSpeed);
            currentTile = current;
            GameController.Instance.CheckMilfedBee();
        }
        
        originTile = targetTile;
        targetTile = null;
        moving = false;
    }
}
