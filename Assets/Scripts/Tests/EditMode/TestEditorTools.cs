using NUnit.Framework;
using UnityEngine.UIElements;
using UnityEditor;

//Unit tests for the editor tools
public class TestEditorTools
{
    [Test]
    public void TestEmpty()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = new VisualElement();
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Settings.IsometricWorldDebugUI);
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Settings.IsometricWorldDebugUIStyles);

        // Adding debug label
        VisualElement templateContainer = visualTree.Instantiate();
        root.Add(templateContainer);
        templateContainer.styleSheets.Add(styleSheet);

        // Setting up Variables
        Button buttonStartDebug = templateContainer.Q<Button>(Settings.DebugStartButton);
        Label gridDebugContent = templateContainer.Q<Label>(Settings.GridDebugContent);
        VisualElement gridDisplay = templateContainer.Q<VisualElement>(Settings.GridDisplay);
        VisualElement mainContainer = templateContainer.Q<VisualElement>(Settings.MainContainer);

        Assert.IsNotNull(visualTree);
        Assert.IsNotNull(styleSheet);
        Assert.IsNotNull(templateContainer);
        Assert.IsNotNull(buttonStartDebug);
        Assert.IsNotNull(gridDebugContent);
        Assert.IsNotNull(gridDisplay);
        Assert.IsNotNull(mainContainer);
    }
}