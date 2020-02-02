using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FTG.AudioController;
using UnityEditor;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public Honeycomb originTile; // Where Bee was standing
    public Honeycomb currentTile; // Where Bee is standing
    public Honeycomb targetTile; // Where Bee should go

    public bool moving = false;
    [SerializeField] Coroutine activeNavigation;
    [SerializeField] private int minPathLengthForSound = 3;
    
    [Header("Animaton")]
    [SerializeField] private float graphicsYOffset;
    [SerializeField] private Transform beeGfx;
    [SerializeField] private Transform shadowGfx;
    [SerializeField] private float shadowHoverScale;
    [SerializeField] private float hoverOffset;
    [SerializeField] private float hoverTime;
    [SerializeField] private float movementSpeed;

    private Transform levitatingObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        beeGfx.localPosition = Vector3.up * graphicsYOffset;
        shadowGfx.localPosition = Vector3.up * graphicsYOffset;
        levitatingObjects = transform.parent.parent;
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
        {
            GameController.Instance.CheckMilfedBee();
            return;
        }

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

        if (path.Count >= minPathLengthForSound)
        {
            AudioController.Instance.PlaySound("BeeLoopSound");
        }
        var startIndex = path.First() == originTile ? 1 : 0;
        currentTile = path[startIndex];

        beeGfx.DOLocalMoveY(graphicsYOffset + hoverOffset, hoverTime);
        shadowGfx.DOScale(Vector3.one * shadowHoverScale, hoverTime);
        AudioController.Instance.TransitionToSnapshot("BeeSnapshot", hoverTime);

        yield return new WaitForSeconds(hoverTime);
        for (int i = startIndex; i < path.Count; i++)
        {
            if (!moving) // moving set to false if milfed by check in milf code
            {
                break;
            }
            var current = path[i];
            currentTile = current;
            var milfed = GameController.Instance.CheckMilfedBee();
            transform.DOLocalMove(current.transform.position - levitatingObjects.position, 1f / movementSpeed);
            yield return new WaitForSeconds(1f / movementSpeed);
            if (milfed || !moving) // moving set to false if milfed by check in milf code
            {
                break;
            }
        }
        
        beeGfx.DOLocalMoveY(graphicsYOffset, hoverTime);
        shadowGfx.DOScale(Vector3.one, hoverTime);
        AudioController.Instance.TransitionToSnapshot("SoundSnapshot", hoverTime);
        yield return new WaitForSeconds(hoverTime);

        AudioController.Instance.StopSound("BeeLoopSound");

        originTile = targetTile;
        targetTile = null;
        moving = false;
        GameController.Instance.BeeFinishedNavigating(originTile);
    }
    
#if UNITY_EDITOR    
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
#endif    
}
