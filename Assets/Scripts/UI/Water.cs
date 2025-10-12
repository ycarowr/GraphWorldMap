using System;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Water
{
    public class Water : MonoBehaviour
    {
        [SerializeField] private GameObject[] water;
        [SerializeField] private GameWorldMap gameWorldMap;

        private void Awake()
        {
            gameWorldMap.OnPostCreate += GenerateWater;
        }

        private void GenerateWater()
        {
            var randomIndex = Random.Range(0, water.Length);
            for (int index = 0; index < water.Length; index++)
            {
                water[index].SetActive(randomIndex == index);
            }
        }
    }
}
