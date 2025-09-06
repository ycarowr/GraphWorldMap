using Tools.WorldMapCreation;
using UnityEngine;

namespace Runtime.WorldMap
{
    public class WorldMapNode : MonoBehaviour
    {
        [SerializeField] private GameObject content;

        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetNode(Node node)
        {
            transform.localPosition = node.WorldPosition;
            spriteRenderer.transform.localScale = node.Size;
        }
    }
}