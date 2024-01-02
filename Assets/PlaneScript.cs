using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    public int frameRate;
    public int startFoxNumber;
    public float foxFOV;
    public float foxViewDistance;
    public float foxSpeed;
    public int startRabbitNumber;
    public float rabbitFOV;
    public float rabbitViewDistance;
    public float rabbitSpeed;
    public int startBushNumber;
    public float bushMaxSize;
    public int timeBetweenBushSpawn;    // Seconds
    private int bushSpawnCounter;
    private float xScale;
    private float zScale;

    // Start is called before the first frame update
    void Start()
    {
        // Set the framerate
        Application.targetFrameRate = frameRate;

        // Get the scales and square
        xScale = Mathf.Pow(transform.localScale.x, 2);
        zScale = Mathf.Pow(transform.localScale.z, 2);

        // Spawn foxes
        for (int i = 0; i < startFoxNumber; i++) {
            spawnFox(xScale, zScale);
        }

        // Spawn rabbits
        for (int i = 0; i < startRabbitNumber; i++) {
            spawnRabbit(xScale, zScale);
        }

        // Spawn bushed
        for (int i = 0; i < startBushNumber; i++) {
            spawnBush(xScale, zScale);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bushSpawnCounter++;
        if (bushSpawnCounter > (timeBetweenBushSpawn * frameRate)) {
            bushSpawnCounter = 0;
            spawnBush(xScale, zScale);
        }
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
        fox.AddComponent<FoxScript>().FOV = foxFOV;
        fox.GetComponent<FoxScript>().ViewDistance = foxViewDistance;
        fox.GetComponent<FoxScript>().Speed = foxSpeed;

        // Add the rigidbody so that it can collide
        fox.AddComponent<Rigidbody>();
    }

    void spawnRabbit(float xScale, float zScale) {
        // Create a new rabbit
        GameObject rabbit = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag and layer
        rabbit.tag = "Rabbit";
        rabbit.layer = LayerMask.NameToLayer("Rabbit");

        // Add the script with variables
        rabbit.AddComponent<RabbitScript>().FOV = rabbitFOV;
        rabbit.GetComponent<RabbitScript>().ViewDistance = rabbitViewDistance;
        rabbit.GetComponent<RabbitScript>().Speed = rabbitSpeed;

        // Start the rabbit at a random position
        rabbit.transform.position = new Vector3(Random.Range(-xScale - 1, xScale - 1), 0.5f, Random.Range(-zScale - 1, zScale - 1));
        
        // Add the rigidbody so that it can collide
        rabbit.AddComponent<Rigidbody>();
    }

    void spawnBush(float xScale, float zScale) {
        // Create a new bush
        GameObject bush = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag
        bush.tag = "Bush";
        bush.layer = LayerMask.NameToLayer("Bush");

        // Add the script
        bush.AddComponent<BushScript>().maxSize = bushMaxSize;

        // Start the rabbit at a random position and small size
        bush.transform.position = new Vector3(Random.Range(-xScale - 1, xScale - 1), 0.5f, Random.Range(-zScale - 1, zScale - 1));
        bush.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Add the rigidbody so that it can collide
        bush.AddComponent<Rigidbody>();
    }
}
