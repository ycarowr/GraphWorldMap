using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class GameObjectFactory : ScriptableObject
    {
        [SerializeField] private GameRegionObjectFactory[] factories;

        public GameObject GetObjectByRegionIndex(int regionIndex)
        {
            var factory = factories[regionIndex];
            var prefab = factory.GetRandom();
            return prefab;
        }
    }
}