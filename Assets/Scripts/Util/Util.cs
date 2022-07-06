using UnityEngine;
// This will contain Utility functions, to create Unity Object and other
public static class Util
{
    private const int sortingLevel = Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL; // Background 

    // Creates a Text object in the scecene
    public static TextMesh CreateTextObject(string name, GameObject parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment)
    {
        GameObject gameObject = new GameObject(name, typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.localPosition = localPosition;
        transform.SetParent(parent.transform, true);
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingLevel;
        return textMesh;
    }

    public static Vector3 GetMouseToWorldPosition()
    {
        Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vector.z = 0;
        return vector;
    }        
}