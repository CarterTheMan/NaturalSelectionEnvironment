using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    public int frameRate;
    public int startFoxNumber;
    public float FoxFOV;
    public float FoxViewDistance;
    public float FoxSpeed;
    public string FoxPreyType;
    public int startRabbitNumber;

    // Start is called before the first frame update
    void Start()
    {
        // Set the framerate
        Application.targetFrameRate = frameRate;

        // Get the scales and square
        float xScale = Mathf.Pow(transform.localScale.x, 2);
        float zScale = Mathf.Pow(transform.localScale.z, 2);

        // Spawn foxes
        for (int i = 0; i < startFoxNumber; i++) {
            spawnFox(xScale, zScale);
        }

        // Spawn rabbits
        for (int i = 0; i < startRabbitNumber; i++) {
            spawnRabbit(xScale, zScale);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnFox(float xScale, float zScale) {
        // Create a new fox
        GameObject fox = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag and layer
        fox.tag = "Fox";
        fox.layer = LayerMask.NameToLayer("Fox");

        // Start the fox at a random position
        fox.transform.position = new Vector3(Random.Range(-xScale - 1, xScale - 1), 0.5f, Random.Range(-zScale - 1, zScale - 1));

        // Add the script with variables
        fox.AddComponent<Detect_Object>().FOV = FoxFOV;
        fox.GetComponent<Detect_Object>().ViewDistance = FoxViewDistance;
        fox.GetComponent<Detect_Object>().Speed = FoxSpeed;
        fox.GetComponent<Detect_Object>().PreyType = FoxPreyType;

        // Add the rigidbody so that it can collide
        fox.AddComponent<Rigidbody>();
    }

    void spawnRabbit(float xScale, float zScale) {
        // Create a new rabbit
        GameObject rabbit = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag and layer
        rabbit.tag = "Rabbit";
        rabbit.layer = LayerMask.NameToLayer("Rabbit");

        // Start the rabbit at a random position
        rabbit.transform.position = new Vector3(Random.Range(-xScale - 1, xScale - 1), 0.5f, Random.Range(-zScale - 1, zScale - 1));
        
        // Add the rigidbody so that it can collide
        rabbit.AddComponent<Rigidbody>();
    }
}
