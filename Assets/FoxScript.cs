using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxScript : MonoBehaviour
{
    // User decide / survivability
    public float FOV;
    public float viewDistance;
    public float speed;
    private float maxHunger = 1500;
    private float hunger;

    // Hunting / Mating
    private string preyType = "Rabbit";
    private GameObject huntedPrey = null;
    private GameObject mate = null;
    private float matingHungerThreshold = 0.5f;

    // Movement
    private int moveTime;
    private int moveCounter;
    private Vector3 turnRate;
    private bool walk = true;
    private int frameRate;

    // Coloring
    private Color mateColor = Color.black;
    private Color huntColor = Color.yellow;
    private Color passiveColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        hunger = maxHunger;
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;

        GetComponent<Renderer>().material.color = passiveColor;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle hunger
        Hunger();

        // If in sight, set the prey
        huntedPrey = FindNextTarget(preyType);

        // If in sight, set a mate
        mate = FindNextTarget(tag);

        // Move towards prey or randomly
        move();
    }   

    // Cost hunger, die if starve
    void Hunger() {
        hunger -= speed;
        if (hunger < 0) {
            Destroy(gameObject);
        }
    }

    // Find the closest target and if in sight, set them as the target
    GameObject FindNextTarget(string searchTag) {
        GameObject finalTarget = null;
        float closestFinalTarget = float.MaxValue;

        // Find target in the right range
        GameObject[] targets = GameObject.FindGameObjectsWithTag(searchTag);
        foreach (GameObject target in targets) {
            Vector3 targetDir = target.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            float distancetoPrey = Vector3.Distance(transform.position, target.transform.position);
            
            // If target is self, ignore
            if (target == this.gameObject) {
                break;
            }
            
            // Is in FOV and within sight distance
            if (angle < FOV && distancetoPrey < viewDistance) {
                bool closer = distancetoPrey < closestFinalTarget;
                
                // Is closest prey
                switch (searchTag) {
                    // Prey
                    case var preyType when preyType == preyType:
                        if (closer) {
                            finalTarget = target;
                            closestFinalTarget = distancetoPrey;
                        }
                        break;
                    
                    // Mate
                    case var tag when tag == tag:
                        bool canSelfMate = hunger > (maxHunger * matingHungerThreshold);
                        bool canTargetMate = target.GetComponent<FoxScript>().hunger > (target.GetComponent<FoxScript>().maxHunger * matingHungerThreshold);
                        if (canSelfMate && canTargetMate && closer) {
                            finalTarget = target;
                            closestFinalTarget = distancetoPrey;
                        }
                        break;

                    // default
                    default:
                        break;
                }
            }
        }

        return finalTarget;        
    }

    // Move towards prey or look around
    void move() {
        // How fast to move forward
        var step = speed * Time.deltaTime;

        // Go to mate, else hungt prey, else passive move
        if (mate != null) {
            GetComponent<Renderer>().material.color = mateColor;

            // Move towards target
            Vector3 targetVector = new Vector3(mate.transform.position.x, 0.5f, mate.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);
        
        } else if (huntedPrey != null) {
            GetComponent<Renderer>().material.color = huntColor;

            // Move towards target
            Vector3 targetVector = new Vector3(huntedPrey.transform.position.x, 0.5f, huntedPrey.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);
        } else {
            GetComponent<Renderer>().material.color = passiveColor;

            // Once time expires, reset
            if (moveCounter >= moveTime) {
                walk = !walk;
                moveCounter = 0;

                // If it should walk, go for 1-10 seconds. Else turn for 3-5 seconds. 
                if (walk) {
                    moveTime = Random.Range(frameRate, 10 * frameRate);
                } else {
                    moveTime = Random.Range(3 * frameRate, 5 * frameRate);
                    turnRate = new Vector3(0, Random.Range(-60.0f, 60.0f), 0);
                }
            }

            moveCounter++;
            // If walking forward, else turning
            if (walk) {
                transform.Translate(Vector3.forward * Time.deltaTime);
            } else {
                transform.Rotate(turnRate * Time.deltaTime);
            }
        }
    }

    // When collide with prey, delete the prey
    private IEnumerator OnCollisionEnter(Collision other) {
        // If collides with mate
        if (other.gameObject.tag == tag) {
            // TODO: Get the wait to work
            // Subtract hunger, erase mate and wait
            hunger -= (int)(maxHunger * matingHungerThreshold);
            mate = null;
            yield return new WaitForSeconds(2);

            // TODO: add new baby fox
        }

        // If collides with prey
        if (other.gameObject.tag == preyType) {
            hunger += (int)(other.gameObject.transform.localScale.x * 1000);
            Destroy(other.gameObject);
        }
    }
}

