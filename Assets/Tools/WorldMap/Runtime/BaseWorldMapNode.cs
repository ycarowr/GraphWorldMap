using TMPro;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public abstract class BaseWorldMapNode : MonoBehaviour
    {
        [SerializeField] private GameObject content;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private TMP_Text titleText;

        public void SetNode(Node node)
        {
            transform.localPosition = node.WorldPosition;
            spriteRenderer.transform.localScale = node.Size;
        }
    }
}