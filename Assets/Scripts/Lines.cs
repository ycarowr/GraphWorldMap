using System;
using System.Collections.Generic;
using ToolsYwr.Patterns.Singleton;
using UnityEngine;

public class Lines : SingletonMB<Lines>
{
    public readonly List<LineRenderer> LineRegistry = new();

    protected override void OnAwake()
    {
        base.OnAwake();
        Clear();
    }

    public LineRenderer AddLine()
    {
        var line = new GameObject("Line");
        line.transform.SetParent(transform);
        var lineRenderer = line.AddComponent<LineRenderer>();
        LineRegistry.Add(lineRenderer);
        return LineRegistry[^1];
    }

    public void Clear()
    {
        foreach (var line in LineRegistry)
        {
            Destroy(line.gameObject);
        }

        LineRegistry.Clear();
    }

    public LineRenderer GetLine(int index)
    {
        return LineRegistry[index];
    }

    public void DrawLineList(Vector3[] positions, Color color, bool isLoop = true)
    {
        if (positions.Length % 2 != 0)
        {
            return;
        }

        for (var index = 0; index < positions.Length - 1; index++)
        {
            var currentPosition = positions[index];
            var nextPosition = positions[index + 1];
            var line = AddLine();
            line.loop = isLoop;
            line.SetPosition(0, currentPosition);
            line.SetPosition(1, nextPosition);
            line.positionCount = 2;
            line.startColor = color;
            line.endColor = color;
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    public void DrawLineStrip(ReadOnlySpan<Vector3> points, Color color, bool isLoop = true)
    {
        var line = AddLine();
        line.loop = isLoop;
        line.positionCount = points.Length;
        line.SetPositions(points.ToArray());
        line.startColor = color;
        line.endColor = color;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
    }
}