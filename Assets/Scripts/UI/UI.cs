using System;
using System.Globalization;
using TMPro;
using Tools.WorldMapCore.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private GameWorldMap gameWorldMap;
        [SerializeField] private Button generate;
        [SerializeField] private WorldMapParameters parameters;
        [SerializeField] private TMP_InputField amountInput;
        [SerializeField] private TMP_InputField amountConnections;
        [SerializeField] private TMP_InputField amountStartInput;
        [SerializeField] private TMP_InputField amountEndInput;
        [SerializeField] private TMP_InputField seedInput;
        [SerializeField] private TMP_InputField worldWidthInput;
        [SerializeField] private TMP_InputField worldHeightInput;
        [SerializeField] private TMP_InputField nodeWidthInput;
        [SerializeField] private TMP_InputField nodeHeightInput;
        [SerializeField] private Toggle toggleIsRandom;
        [SerializeField] private TMP_Dropdown orientationDropdown;
        [SerializeField] private TMP_Dropdown sortMethodDropdown;

        private void Awake()
        {
            gameWorldMap.OnCreate += OnCreate;
            generate.onClick.AddListener(Generate);
            amountInput.onValueChanged.AddListener(SetAmount);
            amountConnections.onValueChanged.AddListener(SetAmountConnections);
            amountStartInput.onValueChanged.AddListener(SetAmountStart);
            amountEndInput.onValueChanged.AddListener(SetAmountEnd);
            seedInput.onValueChanged.AddListener(SetSeed);
            worldWidthInput.onValueChanged.AddListener(SetWidth);
            worldHeightInput.onValueChanged.AddListener(SetHeight);
            nodeWidthInput.onValueChanged.AddListener(SetWidthNode);
            nodeHeightInput.onValueChanged.AddListener(SetHeightNode);
            toggleIsRandom.onValueChanged.AddListener(SetIsRandom);
            orientationDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.OrientationGraph.LeftRight)));
            orientationDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.OrientationGraph.BottomTop)));
            orientationDropdown.onValueChanged.AddListener(SetOrientation);
            sortMethodDropdown.options.Add(new TMP_Dropdown.OptionData(nameof(WorldMapParameters.SortMethod.Axis)));
            sortMethodDropdown.options.Add(new TMP_Dropdown.OptionData(nameof(WorldMapParameters.SortMethod.Distance)));
            sortMethodDropdown.onValueChanged.AddListener(SetSortingMethod);
            RefreshUI();
        }

        private void OnCreate()
        {
            seedInput.text = gameWorldMap.WorldMap.Random.Seed.ToString();
        }

        private void Generate()
        {
            parameters.Refresh();
            RefreshUI();
        }

        private void RefreshUI()
        {
            amountInput.text = parameters.Amount.ToString();
            amountConnections.text = parameters.AmountOfRegionConnections.ToString();
            amountStartInput.text = parameters.AmountStart.ToString();
            amountEndInput.text = parameters.AmountEnd.ToString();
            seedInput.text = parameters.Seed.ToString();
            worldWidthInput.text = parameters.TotalWorldSize.x.ToString(CultureInfo.InvariantCulture);
            worldHeightInput.text = parameters.TotalWorldSize.y.ToString(CultureInfo.InvariantCulture);
            nodeWidthInput.text = parameters.NodeWorldSize.x.ToString(CultureInfo.InvariantCulture);
            nodeHeightInput.text = parameters.NodeWorldSize.y.ToString(CultureInfo.InvariantCulture);
            toggleIsRandom.isOn = parameters.IsRandomSeed;
            orientationDropdown.value = parameters.Orientation == WorldMapParameters.OrientationGraph.BottomTop ? 1 : 0;
            sortMethodDropdown.value = parameters.SortingMethod == WorldMapParameters.SortMethod.Distance ? 1 : 0;
        }

        private void SetSortingMethod(int arg0)
        {
            var value = Enum.Parse<WorldMapParameters.SortMethod>(sortMethodDropdown.options[arg0].text);
            parameters.SortingMethod = value;
        }

        private void SetOrientation(int arg0)
        {
            var value = Enum.Parse<WorldMapParameters.OrientationGraph>(orientationDropdown.options[arg0].text);
            parameters.Orientation = value;
        }

        private void SetIsRandom(bool arg0)
        {
            parameters.IsRandomSeed = arg0;
        }

        private void SetWidthNode(string arg0)
        {
            var current = parameters.NodeWorldSize;
            var width = float.Parse(arg0);
            current.x = width;
            parameters.NodeWorldSize = current;
        }

        private void SetHeightNode(string arg0)
        {
            var current = parameters.NodeWorldSize;
            var height = float.Parse(arg0);
            current.y = height;
            parameters.NodeWorldSize = current;
        }

        private void SetHeight(string arg0)
        {
            var current = parameters.TotalWorldSize;
            var height = float.Parse(arg0);
            current.y = height;
            parameters.TotalWorldSize = current;
        }

        private void SetWidth(string arg0)
        {
            var current = parameters.TotalWorldSize;
            var width = float.Parse(arg0);
            current.x = width;
            parameters.TotalWorldSize = current;
        }

        private void SetSeed(string arg0)
        {
            parameters.Seed = int.Parse(arg0);
        }

        private void SetAmount(string arg0)
        {
            parameters.Amount = int.Parse(arg0);
        }

        private void SetAmountEnd(string arg0)
        {
            parameters.AmountEnd = int.Parse(arg0);
        }

        private void SetAmountStart(string arg0)
        {
            parameters.AmountStart = int.Parse(arg0);
        }

        private void SetAmountConnections(string arg0)
        {
            parameters.AmountOfRegionConnections = int.Parse(arg0);
        }
    }
}