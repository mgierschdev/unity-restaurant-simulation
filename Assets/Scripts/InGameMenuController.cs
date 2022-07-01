using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This class is in charge of controlling the different UI controllers
// Attached to: Canvas UI Object
public class InGameMenuController : MonoBehaviour
{
    GameObject UI;
    GameObject mainMenu;
    public TMP_Text textScore;
    public float score;
    // Start is called before the first frame update
    void Start()
    {
        score = 0f;
        
        textScore.text = "Player score: " + score.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        textScore.text = "Player score: " + score.ToString();

    }
}
