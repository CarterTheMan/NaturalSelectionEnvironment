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
        
        // Set the variables that need to be set unique to animal
        maxHunger = 100;
        hunger = maxHunger;
        mateWaitingPeriod = false;
        eatWaitingPeriod = true;
        preyTag = "Bush";
        predatorTag = "Fox";

        // General variables to be set 
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;
        timeToFlee = timeToFlee * frameRate;
    }
    
}

