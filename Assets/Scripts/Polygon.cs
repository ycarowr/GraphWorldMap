using UnityEngine;

[ExecuteAlways]
public class Polygon : MonoBehaviour
{
    public Vector3[] vertices;
    public LineRenderer lineRenderer;

    public void Awake()
    {
        lineRenderer.SetPositions(vertices);
        lineRenderer.loop = true;
        lineRenderer.positionCount = vertices.Length;
    }

    public void OnEnable()
    {
        lineRenderer.SetPositions(vertices);
        lineRenderer.loop = true;
        lineRenderer.positionCount = vertices.Length;
    }

    //public void OnDrawGizmos()
    //{
    //Gizmos.color = Color.cyan;
    //Gizmos.DrawLineStrip(vertices, true);
    //Gizmos.DrawWireSphere(CalcCentroid(), 1);
    //}


    public Vector3 CalcCentroid()
    {
        if (vertices == null || vertices.Length < 3)
        {
            return Vector3.zero;
        }

        var centroid = Vector3.zero;
        foreach (var v in vertices)
        {
            centroid += v;
        }

        centroid /= vertices.Length;

        return centroid;
    }
}