using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class GameRegionObjectFactory : ScriptableObject
    {
        [SerializeField] private GameObject[] objects;
        public GameObject[] GetObjects() => objects;
        public GameObject GetRandom() => objects[Random.Range(0, objects.Length)];
    }
}