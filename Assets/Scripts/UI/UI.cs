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
        [SerializeField] private Toggle toggleAnimation;
        [SerializeField] private TMP_Dropdown orientationDropdown;
        [SerializeField] private TMP_Dropdown sortMethodDropdown;
        [SerializeField] private TMP_Dropdown debugModeDropdown;
        [SerializeField] private TMP_Dropdown deletionDropdown;

        private void Awake()
        {
            gameWorldMap.OnPostCreate += OnPostCreate;
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
            toggleAnimation.onValueChanged.AddListener(SetAnimation);

            orientationDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.EOrientationGraph.LeftRight)));
            orientationDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.EOrientationGraph.BottomTop)));
            orientationDropdown.onValueChanged.AddListener(SetOrientation);

            sortMethodDropdown.options.Add(new TMP_Dropdown.OptionData(nameof(WorldMapParameters.ESortMethod.Axis)));
            sortMethodDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.ESortMethod.Distance)));
            sortMethodDropdown.onValueChanged.AddListener(SetSortingMethod);

            debugModeDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.DebugData.EDrawMode.None)));
            debugModeDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.DebugData.EDrawMode.Nodes)));
            debugModeDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.DebugData.EDrawMode.Graph)));
            debugModeDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.DebugData.EDrawMode.Distances)));
            debugModeDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.DebugData.EDrawMode.All)));
            debugModeDropdown.onValueChanged.AddListener(SetDebugMode);

            deletionDropdown.options.Add(new TMP_Dropdown.OptionData(nameof(WorldMapParameters.EDeletionReason.None)));
            deletionDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.EDeletionReason.Overlap)));
            deletionDropdown.options.Add(
                new TMP_Dropdown.OptionData(nameof(WorldMapParameters.EDeletionReason.OutOfWorldBounds)));
            deletionDropdown.options.Add(new TMP_Dropdown.OptionData(nameof(WorldMapParameters.EDeletionReason.All)));
            deletionDropdown.onValueChanged.AddListener(SetDeletion);

            RefreshUI();
        }

        private void OnPostCreate()
        {
            seedInput.text = gameWorldMap.WorldMap.Random.Seed.ToString();
            RefreshUI();
        }

        private void Generate()
        {
            parameters.Create();
        }

        private void RefreshUI()
        {
            Debug.Log("OnPostCreate - Refresh UI...");
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
            toggleAnimation.isOn = parameters.IsAnimation;
            orientationDropdown.value = orientationDropdown.options.IndexOf(orientationDropdown.options.Find(x =>
                Enum.Parse<WorldMapParameters.EOrientationGraph>(x.text) == parameters.Orientation));
            sortMethodDropdown.value = sortMethodDropdown.options.IndexOf(sortMethodDropdown.options.Find(x =>
                Enum.Parse<WorldMapParameters.ESortMethod>(x.text) == parameters.SortingMethod));
            debugModeDropdown.value = debugModeDropdown.options.IndexOf(debugModeDropdown.options.Find(x =>
                Enum.Parse<WorldMapParameters.DebugData.EDrawMode>(x.text) == parameters.DebugValues.Mode));
            deletionDropdown.value = deletionDropdown.options.IndexOf(deletionDropdown.options.Find(x =>
                Enum.Parse<WorldMapParameters.EDeletionReason>(x.text) == parameters.DebugValues.DeletionReason));
        }

        private void SetDeletion(int arg0)
        {
            var value = Enum.Parse<WorldMapParameters.EDeletionReason>(deletionDropdown.options[arg0].text);
            parameters.DebugValues.DeletionReason = value;
        }

        private void SetDebugMode(int arg0)
        {
            var value = Enum.Parse<WorldMapParameters.DebugData.EDrawMode>(debugModeDropdown.options[arg0].text);
            parameters.DebugValues.Mode = value;
        }

        private void SetSortingMethod(int arg0)
        {
            var value = Enum.Parse<WorldMapParameters.ESortMethod>(sortMethodDropdown.options[arg0].text);
            parameters.SortingMethod = value;
        }

        private void SetOrientation(int arg0)
        {
            var value = Enum.Parse<WorldMapParameters.EOrientationGraph>(orientationDropdown.options[arg0].text);
            parameters.Orientation = value;
        }

        private void SetAnimation(bool arg0)
        {
            parameters.IsAnimation = arg0;
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