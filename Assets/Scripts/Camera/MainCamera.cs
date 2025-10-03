using Tools.Attributes;
using Tools.WorldMapCore.Database;
using ToolsYwr.Patterns.Singleton;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class MainCamera : SingletonMB<MainCamera>
    {
        private const float SMALL_SPACING = 2;
        [SerializeField] private float zoomSpeed = 200;
        [SerializeField] private Vector3 offset;
        [SerializeField] private GameWorldMap gameWorldMap;
        private bool IsUpdatingZoom { get; set; }
        private float Target { get; set; }
        private Camera CameraComponent { get; set; }

        private void Update()
        {
            if (!IsUpdatingZoom)
            {
                return;
            }

            if (Mathf.Approximately(Target, CameraComponent.orthographicSize))
            {
                IsUpdatingZoom = false;
            }
            else
            {
                CameraComponent.orthographicSize =
                    Mathf.Lerp(CameraComponent.orthographicSize, Target, zoomSpeed * Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            gameWorldMap.OnPostCreate += OnPostCreate;
            if (CameraComponent == null)
            {
                CameraComponent = GetComponent<Camera>();
            }
        }

        private void OnDisable()
        {
            gameWorldMap.OnPostCreate -= OnPostCreate;
        }

        [Button]
        public void OnPostCreate()
        {
            if (!IsValid())
            {
                return;
            }

            Debug.Log("OnPostCreate - Sync Zoom and Camera Position...");
            ResetZoom();
            CentralizePosition();
            SetOrthographicSize();
        }

        private void ResetZoom()
        {
            const float minValue = 10000f;
            CameraComponent.orthographicSize = minValue;
        }

        private void SetOrthographicSize()
        {
            var worldBounds = gameWorldMap.WorldMap.Data.WorldBounds;
            var nodeSize = gameWorldMap.WorldMap.Data.Parameters.NodeWorldSize;
            var worldAspect = CalcWorldAspect(worldBounds, nodeSize);
            CalcOrthographicSize(worldAspect, worldBounds, nodeSize);
            IsUpdatingZoom = true;
        }

        private float CalcWorldAspect(Rect worldBounds, Vector2 nodeSize)
        {
            if (gameWorldMap.WorldMap.Data.Parameters.Orientation == WorldMapParameters.EOrientationGraph.LeftRight)
            {
                return (worldBounds.width + nodeSize.x) / worldBounds.height;
            }

            return worldBounds.width / (worldBounds.height + nodeSize.y);
        }

        private void CalcOrthographicSize(float worldAspect, Rect worldBounds, Vector2 nodeSize)
        {
            if (gameWorldMap.WorldMap.Data.Parameters.Orientation == WorldMapParameters.EOrientationGraph.LeftRight)
            {
                if (worldAspect > CameraComponent.aspect)
                {
                    Target = (worldBounds.width + nodeSize.x * 2) / (CameraComponent.aspect * 2);
                }
                else
                {
                    Target = worldBounds.height / 2f;
                }
            }
            else
            {
                if (worldAspect > CameraComponent.aspect)
                {
                    Target = (worldBounds.width + nodeSize.x * 2) / (CameraComponent.aspect * 2);
                }
                else
                {
                    Target = (worldBounds.height + nodeSize.y * 2) / 2f;
                }
            }

            Target += SMALL_SPACING;
        }

        private void CentralizePosition()
        {
            Vector3 position = gameWorldMap.WorldMap.Data.WorldBounds.center;
            transform.position = position + offset;
        }

        private bool IsValid()
        {
            return gameWorldMap.WorldMap != null;
        }
    }
}