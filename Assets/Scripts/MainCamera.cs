using System;
using UnityEngine;

namespace Game
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private GameWorldMap gameWorldMap;

        private void Awake()
        {
            gameWorldMap.OnCreate += OnCreateWorldMap;
        }

        private void OnDestroy()
        {
            gameWorldMap.OnCreate -= OnCreateWorldMap;
        }

        private void OnCreateWorldMap()
        {
            Vector3 position = gameWorldMap.WorldMap.Data.WorldBounds.center;
            transform.position = position + offset;
            GetComponent<Camera>().orthographicSize = gameWorldMap.WorldMap.Data.WorldBounds.size.y / 2;
        }
    }
}
