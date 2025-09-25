using System;
using TMPro;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Game.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private WorldMapParameters parameters;
        [SerializeField] private TMP_InputField amountInput;
        [SerializeField] private TMP_InputField seedInput;

        private void Awake()
        {
            amountInput.text = parameters.Amount.ToString();
            amountInput.onValueChanged.AddListener(SetAmount);
            
            seedInput.text = parameters.Seed.ToString();
            seedInput.onValueChanged.AddListener(SetSeed);
        }

        private void SetSeed(string msg)
        {
            parameters.Seed = int.Parse(msg);
        }

        private void SetAmount(string msg)
        {
            parameters.Amount = int.Parse(msg);
        }
    }
}
