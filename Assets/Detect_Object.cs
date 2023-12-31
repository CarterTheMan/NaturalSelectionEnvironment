using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect_Object : MonoBehaviour
{
    // User decide
    public int frameRate;
    public float FOV;
    public float ViewDistance;
    public float speed;
    public string PreyType;
    private GameObject huntedPrey;
    private int moveTime;
    private int moveCounter;
    private Vector3 turnRate;
    private bool walk;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = frameRate;
        GetComponent<Renderer>().material.color = Color.red;
        huntedPrey = null;
        walk = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the prey
        FindClosestPrey();

        // Move towards prey or randomly
        move();
    }   

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

    void move() {
        // How fast to move forward
        var step = speed * Time.deltaTime;

        // Hunt prey
        if (huntedPrey != null) {
            GetComponent<Renderer>().material.color = Color.green;

            // Move towards target
            // NOTE: Need to retate quickly to look at
            transform.position = Vector3.MoveTowards(transform.position, huntedPrey.transform.position, step);

        // Not hunting prey
        } else {
            GetComponent<Renderer>().material.color = Color.red;

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
}

