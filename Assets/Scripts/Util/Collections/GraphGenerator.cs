using System;
using System.Collections.Generic;
using UnityEngine;

// Draws a Graph given an adgency matrix and an enum representing the names of the nodes
public class GraphGenerator<T> where T : Enum
{
    private int[,] adjencyMatrix;
    private Enum nodeNames;

    public GraphGenerator(int[,] adjencyMatrix)
    {
        this.adjencyMatrix = adjencyMatrix;
    }

    public void Draw()
    {

        for (int i = 0; i < adjencyMatrix.GetLength(0); i++)
        {
            string s = "";
            Debug.Log(GetNodeName(i));
            for (int j = 0; j < adjencyMatrix.GetLength(1); j++)
            {
                s += " " + adjencyMatrix[i, j];
            }
            Debug.Log(s);
        }
    }

    private string GetNodeName(int i)
    {
        return ((T)Enum.ToObject(typeof(T), i)).ToString();
    }
}