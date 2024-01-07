using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitScript : AnimalScript
{
    
    // Start is called before the first frame update
    void Start()
    {
        // Coloring
        mateColor = Color.blue;       // cyan
        huntColor = Color.blue;       // magenta
        passiveColor = Color.blue;    // blue
        GetComponent<Renderer>().material.color = passiveColor;
        
        // Set the variables that need to be set
        hunger = maxHunger;
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;
        preyTag = "Bush";
    }
    
}

