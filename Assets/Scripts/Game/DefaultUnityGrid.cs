using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnityGrid : MonoBehaviour
{
    private Grid grid;
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetGridPosition(Vector3 position){
        return grid.WorldToCell(position);
    }
}