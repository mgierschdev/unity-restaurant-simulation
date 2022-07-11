using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFind
{
    private const int COST_DIAGONAL = 14;
    private const int COST_STRAIGHT = 10;

    private PathNode[,] grid;
    private bool found;
    private int[] target;
    private int[] start;
    private List<int[]> path;

    public List<int[]> Find(int[] start, int[] target, int[,] sourceGrid){

        found = false;
        this.target = target;
        this.start = start;
        path = new List<int[]>();
        
        InitGridPathNode(sourceGrid);

        DFS(start[0], start[1], null);

        PrintGridPathNodes();
        // DFS(start[0], start[1], 0);
        return path;
    }

    private void DFS(int x, int y, PathNode parent){
        if(IsValid(x, y) && grid[x, y].GetValue() == 0 && !found){
            

            PathNode current = grid[x, y];
            current.SetParent(parent);
            current.SetValue(2);
            
            
            if(x == target[0] && y == target[1]){
                found = true;
            }

            PriorityQueue queue = new PriorityQueue();
            queue.Enqueue(new int[]{x + 1, y}, CalculateF(new int[]{x + 1, y}));
            queue.Enqueue(new int[]{x - 1, y}, CalculateF(new int[]{x - 1, y}));
            queue.Enqueue(new int[]{x, y + 1}, CalculateF(new int[]{x, y + 1}));
            queue.Enqueue(new int[]{x, y - 1}, CalculateF(new int[]{x, y - 1}));
            queue.Enqueue(new int[]{x + 1, y - 1}, CalculateF(new int[]{x + 1, y - 1}));
            queue.Enqueue(new int[]{x - 1, y + 1}, CalculateF(new int[]{x - 1, y + 1}));
            queue.Enqueue(new int[]{x - 1, y - 1}, CalculateF(new int[]{x - 1, y - 1}));
            queue.Enqueue(new int[]{x + 1, y + 1}, CalculateF(new int[]{x + 1, y + 1}));

            while(!queue.IsEmpty()){
                QueueNode n =  queue.Dequeue();
                int[] coord = n.GetData();
                DFS(coord[0], coord[1], current); 
            }
        }
    }

    private bool IsValid(int x, int y){
        return !(x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1));
    }

    private void InitGridPathNode(int[,] clone){
        grid = new PathNode[clone.GetLength(0), clone.GetLength(0)];
        for(int i = 0; i < clone.GetLength(0); i++){
            for(int j = 0; j < clone.GetLength(1); j++){
                int[] coord = new int[]{i, j};
                grid[i, j] = new PathNode(coord, CalculateDistance(coord, start), int.MaxValue, clone[i, j]);
            }
        }
    }

    public int CalculateF(int[] a){
        return CalculateDistance(a, target) + CalculateDistance(a, start);
    }

    // Return the min distance without obstacles between 2 points
    public int CalculateDistance(int[] a, int[] b){
        int xAbs = Mathf.Abs(a[0] - b[0]);
        int yAbs = Mathf.Abs(a[1] - b[1]);
        int diff = Mathf.Abs(xAbs - yAbs);

        return COST_DIAGONAL * Mathf.Min(xAbs, yAbs) + COST_STRAIGHT * diff;
    }

    // Full grid debug
    private void PrintGridPathNodes(){
        // Debug.Log("...... GetEuclidianCost");
        // for(int i = 0 ; i < grid.GetLength(0); i++){
        //     String s = "";
        //     for(int j = 0; j < grid.GetLength(1); j++){
        //         s += " "+Math.Truncate(grid[i, j].GetEuclidianCost());
        //     }
        //     Debug.Log(s);
        // }
        Debug.Log("GetValue");
        for(int i = 0 ; i < grid.GetLength(0); i++){
            String s = "";
            for(int j = 0; j < grid.GetLength(1); j++){
                s += "  .  "+grid[i, j].GetValue();
            }
            Debug.Log(s);
        }
        // Debug.Log("...... H");
        // for(int i = 0 ; i < grid.GetLength(0); i++){
        //     String s = "";
        //     for(int j = 0; j < grid.GetLength(1); j++){
        //         s += " "+grid[i, j].GetCostEnd();
        //     }
        //     Debug.Log(s);
        // }
        
        // Debug.Log("...... G");
        // for(int i = 0 ; i < grid.GetLength(0); i++){
        //     String s = "";
        //     for(int j = 0; j < grid.GetLength(1); j++){
        //         s += " "+grid[i, j].GetCostToStart();
        //     }
        //     Debug.Log(s);
        // }
    }
}
