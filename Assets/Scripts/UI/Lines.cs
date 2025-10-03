using System;
using System.Collections.Generic;
using Tools.WorldMapCore.Database;
using ToolsYwr.Patterns.Singleton;
using UnityEngine;

public class Lines : SingletonMB<Lines>
{
    private const string LineName = "Line";
    private const string ShaderName = "Sprites/Default";
    private static Shader DefaultShader;

    [SerializeField] private WorldMapParameters parameters;

    private readonly List<LineRenderer> LineRegistry = new();

    private float LineSize { get; set; }

    private static Material CreateMaterial()
    {
        return new Material(DefaultShader);
    }

    protected override void OnAwake()
    {
        DefaultShader = Shader.Find(ShaderName);
        Clear();
    }

    private LineRenderer AddLine()
    {
        var line = new GameObject(LineName);
        line.transform.SetParent(transform);
        var lineRenderer = line.AddComponent<LineRenderer>();
        LineRegistry.Add(lineRenderer);
        return LineRegistry[^1];
    }

    public void Clear()
    {
        LineSize = Instance.parameters.LineSize;
        foreach (var line in LineRegistry)
        {
            Destroy(line.gameObject);
        }

        LineRegistry.Clear();
    }

    public static void DrawLineList(Vector3[] positions, Color color)
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = color;
            Gizmos.DrawLineList(positions);
            return;
        }

        if (positions.Length % 2 != 0)
        {
            return;
        }

        for (var index = 0; index < positions.Length - 2; index += 2)
        {
            var currentPosition = positions[index];
            var nextPosition = positions[index + 1];
            var line = Instance.AddLine();
            line.SetPosition(0, currentPosition);
            line.SetPosition(1, nextPosition);
            line.positionCount = 2;
            line.startColor = color;
            line.endColor = color;
            line.startWidth = Instance.LineSize;
            line.endWidth = Instance.LineSize;
            line.material = CreateMaterial();
        }
    }

    public static void DrawLineStrip(ReadOnlySpan<Vector3> positions, Color color, bool isLoop = true)
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = color;
            Gizmos.DrawLineStrip(positions, isLoop);
            return;
        }

        var line = Instance.AddLine();
        line.loop = isLoop;
        line.positionCount = positions.Length;
        line.SetPositions(positions.ToArray());
        line.startColor = color;
        line.endColor = color;
        line.startWidth = Instance.LineSize;
        line.endWidth = Instance.LineSize;
        line.material = CreateMaterial();
    }
}