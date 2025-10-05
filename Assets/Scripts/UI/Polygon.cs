using System;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

[ExecuteAlways]
public class Polygon : MonoBehaviour
{
    public Rect one;
    public Rect two;
    public Rect three;
    public Rect four;
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(one.center, one.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(two.center, two.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(three.center, three.size);
        four = WorldMapHelper.FindRectIntersection(one, two);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(four.center, four.size);
    }

    public void Update()
    {
        Debug.Log(WorldMapHelper.CheckOverlapX(one, two) + " " + WorldMapHelper.CheckOverlapY(one, two));
    }

    
}