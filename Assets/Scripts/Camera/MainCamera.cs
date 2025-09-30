using Tools.Attributes;
using Tools.WorldMapCore.Database;
using ToolsYwr.Patterns.Singleton;
using UnityEngine;

namespace Game
{
    public class MainCamera : SingletonMB<MainCamera>
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private GameWorldMap gameWorldMap;

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
            var cameraComponent = GetComponent<Camera>();
            if (gameWorldMap.WorldMap.Data.Parameters.Orientation == WorldMapParameters.EOrientationGraph.LeftRight)
            {
                if (worldAspect > cameraComponent.aspect)
                {
                    cameraComponent.orthographicSize =
                        (worldBounds.width + nodeSize.x * 2) / (cameraComponent.aspect * 2);
                }
                else
                {
                    cameraComponent.orthographicSize = worldBounds.height / 2f;
                }
            }
            else
            {
                if (worldAspect > cameraComponent.aspect)
                {
                    cameraComponent.orthographicSize =
                        (worldBounds.width + nodeSize.x * 2) / (cameraComponent.aspect * 2);
                }
                else
                {
                    cameraComponent.orthographicSize = (worldBounds.height + nodeSize.y * 2) / 2f;
                }
            }
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