using TMPro;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public abstract class BaseWorldMapNode : MonoBehaviour
    {
        [SerializeField] private GameObject content;

        [SerializeField] private TMP_Text titleText;

        public void SetNode(Node node)
        {
            titleText.text = node.ID.ToString();
            transform.localPosition = node.WorldPosition;
        }
    }
}