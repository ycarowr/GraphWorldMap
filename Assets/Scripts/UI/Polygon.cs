using Tools.WorldMapCore.Runtime;
using UnityEngine;

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
}