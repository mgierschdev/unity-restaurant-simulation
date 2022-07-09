using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFind
{
    private PathNode[,] grid;
    private bool found;
    private int[] target;
    private List<int[]> path;
    private double cost;

    public List<int[]> Find(int[] start, int[] target, int[,] sourceGrid){
        // We init vars
        //grid = Util.CloneGrid(sourceGrid); // We create a clone to not modify the original
        found = false;
        this.target = target;
        path = new List<int[]>();
        cost = 0.0f;
        InitGridPathNode(sourceGrid);

        DFS(start[0], start[1],  0.0f);
        //Util.PrintGrid(grid);
        return path;
 
    }

    private bool DFS(int x, int y, double cost){
        if(IsValid(x, y) && grid[x, y] == 0 && !found){

            int[] currentNode = new int[]{x, y};
            route.Add(currentNode);
            grid[x, y] = Util.EuclidianDistance(currentNode, target);

            if(x == target[0] && y == target[1]){
                path = new List<int[]>(route);
                this.cost = cost;
                found = true;
                return true;
            }

            PriorityQueue queue = new PriorityQueue();
            queue.Enqueue(new int[]{x + 1, y}, Util.EuclidianDistance(new int[]{x + 1, y}, target));
            queue.Enqueue(new int[]{x - 1, y}, Util.EuclidianDistance(new int[]{x - 1, y}, target));
            queue.Enqueue(new int[]{x, y + 1}, Util.EuclidianDistance(new int[]{x, y + 1}, target));
            queue.Enqueue(new int[]{x, y - 1}, Util.EuclidianDistance(new int[]{x, y - 1}, target));
            queue.Enqueue(new int[]{x + 1, y - 1}, Util.EuclidianDistance(new int[]{x + 1, y - 1}, target));
            queue.Enqueue(new int[]{x - 1, y + 1}, Util.EuclidianDistance(new int[]{x - 1, y + 1}, target));
            queue.Enqueue(new int[]{x - 1, y - 1}, Util.EuclidianDistance(new int[]{x - 1, y - 1}, target));
            queue.Enqueue(new int[]{x + 1, y + 1}, Util.EuclidianDistance(new int[]{x + 1, y + 1}, target));


            while(!queue.IsEmpty()){
                QueueNode n =  queue.Dequeue();
                int[] coord = n.GetData();
                if(!DFS(coord[0], coord[1], cost + grid[x, y], route)){
                  route.Remove(currentNode);
                }   
            }
        }
        return false;
    }

    private bool IsValid(int x, int y){
        return !(x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1));
    }

    private void InitGridPathNode(int[,] clone){
        grid = new PathNode[clone.GetLength(0), clone.GetLength(0)];
        for(int i = 0; i < clone.GetLength(0); i++){
            for(int j = 0; j < clone.GetLength(1); j++){
                double[] coord = new double[]{clone[i, 0], clone[i, 0]};
                grid[i, j] = new PathNode(coord, 0 ,Util.CalculateSqareDistance(coord, target), Util.EuclidianDistance(coord, target));
            }
        }
    }

    public double GetCost(){
        return cost;
    }

}
