using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFind
{
    private double[,] grid;
    private bool found;

    public List<int[]> Find(int[] start, int[] target, double[,] sourceGrid){
        grid = Util.CloneGrid(sourceGrid); // We create a clone to not modify the original
        found = false;
        return DFS(start[0], start[1], target, 0.0f, new List<int[]>());
    }

    private List<int[]> DFS(int x, int y, int[] target, double cost, List<int[]> route){
        if(IsValid(x, y) && grid[x, y] == 0 && !found){
            if(x == target[0] && y == target[1]){
                found = true;
                return route;
            }

            Debug.Log(route);
            int[] currentNode = new int[]{x, y};
            route.Add(currentNode);
            grid[x, y] = Util.EuclidianDistance(currentNode, target);
            SortedList<int[], double> sortedList = new SortedList<int[], double>();

            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x + 1, y}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x - 1, y}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x, y + 1}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x, y - 1}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x + 1, y - 1}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x - 1, y + 1}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x - 1, y - 1}, target));
            sortedList.Add(currentNode, Util.EuclidianDistance(new int[]{x + 1, y + 1}, target));

            // while(sortedList.Count > 0){
            //     int[] element = (int[]) sortedList.Keys[0];
            //     sortedList.RemoveAt(0);
            //  //   Debug.Log(element);
            // }
        }
        return route;
    }

    private bool IsValid(int x, int y){
        return !(x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1));
    }

}
