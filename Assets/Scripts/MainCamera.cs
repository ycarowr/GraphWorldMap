using System;
using Tools.Attributes;
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

        [Button]
        public void OnCreateWorldMap()
        {
            var cameraComponent = GetComponent<Camera>();
            Vector3 position = gameWorldMap.WorldMap.Data.WorldBounds.center;
            transform.position = position + offset;

            if (gameWorldMap.WorldMap.Data.WorldBounds.width > gameWorldMap.WorldMap.Data.WorldBounds.height)
            {
                cameraComponent.orthographicSize = gameWorldMap.WorldMap.Data.WorldBounds.width / 3;
            }
            else
            {
                cameraComponent.orthographicSize = gameWorldMap.WorldMap.Data.WorldBounds.height / 2;   
            }
        }
    }
}
