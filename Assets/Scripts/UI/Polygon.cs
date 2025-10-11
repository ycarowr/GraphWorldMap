using System;
using System.Linq;
using Tools.WorldMapCore.Runtime;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// /// (x:144.616837, y:110.736389, width:39.2478371, height:35.2059364)
/// (x:43.6880569, y:138.950775, width:49.7264061, height:19.8072186)
/// (x:146.974487, y:108.479897, width:22.2342281, height:45.3999443)
/// </summary>
[ExecuteAlways]
public class Polygon : MonoBehaviour
{
    public Rect one;
    public Rect two;
    public Rect three;
    public Rect four;
    public Rect five;

    public Rect[] adjacents;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(one.center, one.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(two.center, two.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(three.center, three.size);
        four = WorldMapHelper.FindRectIntersection(one, two);
        five = WorldMapHelper.FindRectIntersection(one, three);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(four.center, four.size);
        Gizmos.DrawWireCube(five.center, five.size);

        adjacents = WorldMapHelper.FindAdjacentRects(2, one, new[]
        {
            two, three,
        });
    }

    
    public void Update()
    {
        //Debug.Log(one.IsOnTop(two));
    }
}