using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathfinderTest : MonoBehaviour
{
    public Honeycomb from;
    public Honeycomb to;
    
#if UNITY_EDITOR
    [CustomEditor(typeof(PathfinderTest))]
    public class PathfinderBeemakerEditor : Editor
    {
        private Object from;
        private Object to;
        
        public override void OnInspectorGUI()
        {
            var test = target as PathfinderTest;
            DrawDefaultInspector();
            
            if (GUILayout.Button("FindPath")) //10
            {
                Debug.Log(test.from?.number + " to " + test.to?.number);
                var path = PathfinderBeemaker.FindPath(test.from, test.to);

                if (path.Count == 0)
                {
                    Debug.Log("No Path found");
                }
                else
                {
                    var str = string.Join("-", path.Select(x => x.number));
                    Debug.Log(str);
                }
            }
        }
    }
#endif
}
