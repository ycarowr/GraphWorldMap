using Tools.Attributes;
using Tools.WorldMapCore.Database;
using ToolsYwr.Patterns.Singleton;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class MainCamera : SingletonMB<MainCamera>
    {
        private const float SMALL_SPACING = 2;
        [SerializeField] private float zoomSpeed = 200;
        [SerializeField] private Vector3 offset;
        [SerializeField] private GameWorldMap gameWorldMap;
        private float Target { get; set; }
        private Camera CameraComponent { get; set; }

        private void Update()
        {
            if (!Mathf.Approximately(Target, CameraComponent.orthographicSize))
            {
                CameraComponent.orthographicSize =
                    Mathf.Lerp(CameraComponent.orthographicSize, Target, zoomSpeed * Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            if (CameraComponent == null)
            {
                CameraComponent = GetComponent<Camera>();
            }
        }

        protected override void OnDestroy()
        {
            gameWorldMap.OnCreate -= OnCreateWorldMap;
            base.OnDestroy();
        }

        protected override void OnAwake()
        {
            gameWorldMap.OnCreate += OnCreateWorldMap;
        }

        [Button]
        public void OnCreateWorldMap()
        {
            if (!IsValid())
            {
                return;
            }

            CentralizePosition();
            SetOrthographicSize();
        }

        private void SetOrthographicSize()
        {
            var worldBounds = gameWorldMap.WorldMap.Data.WorldBounds;
            var nodeSize = gameWorldMap.WorldMap.Data.Parameters.NodeWorldSize;
            var worldAspect = CalcWorldAspect(worldBounds, nodeSize);
            CalcOrthographicSize(worldAspect, worldBounds, nodeSize);
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
            const float minValue = 10000f; 
            CameraComponent.orthographicSize = minValue;
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