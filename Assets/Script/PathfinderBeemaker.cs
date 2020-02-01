using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class PathfinderBeemaker
{
    public static List<Honeycomb> FindPath(Honeycomb start, Honeycomb end, int maxSteps = 500)
    {
        if (start == null || end == null)
        {
            return new List<Honeycomb>();
        }
        
        var parents = new Dictionary<Honeycomb, Honeycomb>();
        var q = new Queue<Honeycomb>();
        var discovered = new HashSet<Honeycomb>();

        discovered.Add(start);
        q.Enqueue(start);

        int steps = 1;
        while (steps < maxSteps && q.Count > 0)
        {
            var current = q.Dequeue();
            if (current == end)
            {
                return BuildListFromParentPath(current, parents);
            }

            foreach (var next in current.GetConnectedHoneycombs())
            {
                if (next != null && !discovered.Contains(next))
                {
                    discovered.Add(next);
                    parents[next] = current;
                    q.Enqueue(next);
                }
            }
            steps++;
        }
        
        // Not found or exceeded max Steps. Should not occur if tiles can only be placed "door on door" and are
        // therefore always connected.

        return new List<Honeycomb>(); // DUMMY
    }

    private static List<Honeycomb> BuildListFromParentPath(Honeycomb end, Dictionary<Honeycomb, Honeycomb> parents)
    {
        var path = new List<Honeycomb>();
        path.Add(end);

        var current = end;
        while (parents.ContainsKey(current))
        {
            var parent = parents[current];
            path.Add(parent);
            current = parent;
        }

        path.Reverse();
        return path;
    }
}
