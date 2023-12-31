using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create a new cube primitive to set the color on
       GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

       // Get the Renderer component from the new cube
       var cubeRenderer = cube.GetComponent<Renderer>();

       // Create a new RGBA color using the Color constructor and store it in a variable
        Color customColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

       // Call SetColor using the shader property name "_Color" and setting the color to the custom color you created
       cubeRenderer.material.SetColor("_Color", customColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
