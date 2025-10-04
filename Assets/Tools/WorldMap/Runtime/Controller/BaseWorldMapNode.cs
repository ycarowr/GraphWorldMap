using TMPro;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public abstract class BaseWorldMapNode : MonoBehaviour
    {
        private const string START = "START\n";
        private const string END = "END\n";
        
        [SerializeField] private GameObject content;
        [SerializeField] private TMP_Text titleText;

        public bool IsStarting { get; set; }
        public bool IsEnding { get; set; }
        
        public void SetNode(WorldMapNode node, int indexColor)
        {
            titleText.text = node.ID.ToString();
            transform.localPosition = node.Center;
            if (IsStarting)
            {
                titleText.text = START + titleText.text;
                titleText.color = WorldMapGraphGizmos.Colors[indexColor];
            }
            if (IsEnding)
            {
                titleText.text = END + titleText.text;
                titleText.color = Color.grey;
            }
        }
    }
}