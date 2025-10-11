using TMPro;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public abstract class BaseWorldMapNode : MonoBehaviour
    {
        private const string START = "START\n";
        private const string END = "END\n";
        public static int IndexColor;
        [SerializeField] private GameObject content;
        [SerializeField] private TMP_Text titleText;

        public bool IsStarting { get; set; }
        public bool IsEnding { get; set; }

        public void SetNode(WorldMapNode node)
        {
            titleText.text = node.ID.ToString();
            transform.localPosition = node.Bound.center;
            if (IsStarting)
            {
                titleText.text = START + titleText.text;
                titleText.color = WorldMapGraphGizmos.Colors[IndexColor];
                IndexColor++;
            }

            if (IsEnding)
            {
                titleText.text = END + titleText.text;
                titleText.color = Color.grey;
            }
        }
    }
}