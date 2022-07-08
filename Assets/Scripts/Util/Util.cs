using System;
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

    public static Vector3 GetMouseInWorldPosition()
    {
        Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vector.z = 0;
        return vector;
    }  

    public static Vector2Int GetXYInGameMap(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x - Settings.GRID_START_X), Mathf.FloorToInt(position.y - Settings.GRID_START_Y));
    }
    
    public static void PrintArray(double[,] grid){
        for(int i = 0; i < grid.GetLength(0); i++){
            String row = "";
            for(int j = 0; j < grid.GetLength(1); j++){
                row += grid[i, j]+" ";
            }
            Debug.Log(row);
        }
        Debug.Log(" ");
    }

    public static double EuclidianDistance(int[] a, int[] b){
        double distance = System.Math.Sqrt(System.Math.Pow(a[0] - b[0], 2) + System.Math.Pow(a[1] - b[1], 2));
        return distance;
    }

    public static double[,] CloneGrid(double[,] grid){
        double[,] newGrid = new double[grid.GetLength(0), grid.GetLength(1)];
        for(int i = 0; i < grid.GetLength(0); i++){
            for(int j = 0; j < grid.GetLength(1); j++){
                newGrid[i, j] = grid[i, j];
            }
        }
        return newGrid;
    }
}