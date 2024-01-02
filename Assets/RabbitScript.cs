using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitScript : MonoBehaviour
{
    // User decide
    public float FOV;
    public float ViewDistance;
    public float Speed;
    private string PreyType;
    private GameObject huntedPrey;
    private int moveTime;
    private int moveCounter;
    private Vector3 turnRate;
    private bool walk;
    private int frameRate;
    private Color huntColor;
    private Color passiveColor;

    // Start is called before the first frame update
    void Start()
    {
        huntColor = Color.magenta;
        passiveColor = Color.blue;
        PreyType = "Bush";

        GetComponent<Renderer>().material.color = passiveColor;
        
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        huntedPrey = null;
        walk = true;
    }

    // Update is called once per frame
    void Update()
    {
        // If in sight, set the prey
        FindClosestPrey();

        // Move towards prey or randomly
        move();
    }   

    // Find the closest prey and if in sight, set them as the prey
    void FindClosestPrey() {
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        // Find preys in the right range
        GameObject[] preys = GameObject.FindGameObjectsWithTag(PreyType);
        foreach (GameObject prey in preys) {
            Vector3 targetDir = prey.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            // Is in FOV
            if (angle < FOV) {
                float distancetoPrey = Vector3.Distance(transform.position, prey.transform.position);
                
                // Is within sight distance
                if (distancetoPrey < ViewDistance) {
                    
                    // Is closest prey
                    if (closest == null || distancetoPrey < closestDistance) {
                        closest = prey;
                        closestDistance = distancetoPrey;
                    }
                }
            }

            // If prey found, return it
            if (closest != null) {
                huntedPrey = closest;

            // If no prey, return null
            } else {
                huntedPrey = null;
            }
        }
    }

    // Move towards prey or look around
    void move() {
        // How fast to move forward
        var step = Speed * Time.deltaTime;

        // Hunt prey
        if (huntedPrey != null) {
            GetComponent<Renderer>().material.color = huntColor;

            // Move towards target
            Vector3 targetVector = new Vector3(huntedPrey.transform.position.x, 0.5f, huntedPrey.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);

        // Not hunting prey
        } else {
            GetComponent<Renderer>().material.color = passiveColor;

            // Once time expires, reset
            if (moveCounter >= moveTime) {
                walk = !walk;
                moveCounter = 0;

                if (walk) {
                    // Walk for 1-10 seconds
                    moveTime = Random.Range(frameRate, 10 * frameRate);
                } else {
                    // Turn for 3-5 seconds
                    moveTime = Random.Range(3 * frameRate, 5 * frameRate);
                    turnRate = new Vector3(0, Random.Range(-60.0f, 60.0f), 0);
                }
            }

            moveCounter++;
            // If walking forward
            if (walk) {
                transform.Translate(Vector3.forward * Time.deltaTime);
            // If turning
            } else {
                transform.Rotate(turnRate * Time.deltaTime);
            }
        }
    }

    // When collide with prey, delete the prey
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == PreyType) {
            Destroy(other.gameObject);
        }
    }
}

