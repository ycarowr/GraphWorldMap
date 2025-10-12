using Game;
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
        [SerializeField] private GameObjectFactory factory;
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

            var prefab = factory.GetObjectByRegionIndex(node.RegionID);
            if (prefab != null)
            {
                var obj = Instantiate(prefab, content.transform);
                obj.transform.localPosition = Vector3.zero;
            }
        }
    }
}