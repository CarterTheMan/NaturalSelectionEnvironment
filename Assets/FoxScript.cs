using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxScript : MonoBehaviour
{
    // User decide / survivability
    public float FOV;
    public float viewDistance;
    public float speed;
    private float maxHunger = 30;     // in speed over size per second
    private float hunger;

    // Growing
    private bool mature = false;
    private float size = 0.25f;
    private float sizePerSecond;
    private float timeToMature = 10;

    // Hunting / Mating
    private const string preyType = "Rabbit";
    private GameObject huntedPrey = null;
    private GameObject mate = null;
    private float matingHungerThreshold = 0.5f;

    // Waiting
    private bool waitBool = false;
    private float waitTotalTime;
    private float waitCounter;

    // Movement
    private int moveTime;
    private int moveCounter;
    private Vector3 turnRate;
    private bool walk = true;
    private int frameRate;

    // Coloring
    private Color mateColor = Color.red;        // black
    private Color huntColor = Color.red;        // yellow
    private Color passiveColor = Color.red;     // red

    // Start is called before the first frame update
    void Start()
    {
        hunger = maxHunger;
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;

        GetComponent<Renderer>().material.color = passiveColor;
    }

    // Update is called once per frame
    void Update()
    {
        // If not in a waiting state, continue as normal
        if (!waitBool) {
            // If not mature, grow
            if (!mature) {
                Grow();
            }

            // Handle hunger
            Hunger();

            // If in sight, look for prey
            huntedPrey = See(preyType);

            // If in sight and mature, look for a mate
            if (mature) {
                mate = See(tag);
            }

            // Move towards mate then prey then randomly
            move();
        } else {
            waitCounter++;
            if (waitCounter > waitTotalTime) {
                waitCounter = 0;
                waitBool = false;
            }
        }
    }   

    // Grow and increase in size   
    void Grow() {
        size += sizePerSecond;
        transform.localScale = new Vector3(size, size, size);
        if (size >= 1) {
            mature = true;
        }
    }

    // Cost hunger, die if starve
    void Hunger() {
        // hunger decreases at speed per second
        hunger -= speed / frameRate;
        if (hunger < 0) {
            Destroy(gameObject);
        }
    }

    // Find the closest target and if in sight, set them as the target
    GameObject See(string searchTag) {
        GameObject finalTarget = null;
        float closestFinalTarget = float.MaxValue;

        // Skip this if searching for mate and too hungry
        if (searchTag == tag && hunger <= (maxHunger * matingHungerThreshold)) {
            return finalTarget;
        }
        
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
                    case preyType:
                        if (closer) {
                            finalTarget = target;
                            closestFinalTarget = distancetoPrey;
                        }
                        break;
                    
                    // Mate
                    case "Fox":
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
        // How fast to move forward, scales with size
        var step = speed * size * Time.deltaTime;

        // Go to mate, else hunt prey, else passive move
        if (mate != null) {
            GetComponent<Renderer>().material.color = mateColor;

            // Move towards target
            Vector3 targetVector = new Vector3(mate.transform.position.x, transform.position.y, mate.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);
        
        } else if (huntedPrey != null) {
            GetComponent<Renderer>().material.color = huntColor;

            // Move towards target
            Vector3 targetVector = new Vector3(huntedPrey.transform.position.x, transform.position.y, huntedPrey.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);
        } else {
            GetComponent<Renderer>().material.color = passiveColor;

            // Once time expires, reset
            if (moveCounter >= moveTime) {
                swapWalkAndTurn();
            }

            moveCounter++;
            // If walking forward, else turning
            if (walk) {
                transform.Translate(Vector3.forward * step);
            } else {
                transform.Rotate(turnRate * Time.deltaTime);
            }
        }
    }

    // If walking, then turn. If turning, then walk
    void swapWalkAndTurn() {
        walk = !walk;
        moveCounter = 0;

        // If it should walk, go for 1-10 seconds. Else turn for 3-5 seconds. 
        if (walk) {
            moveTime = Random.Range(frameRate, 10 * frameRate);
        } else {
            int minTime = 3;
            int maxTime = 5;
            moveTime = Random.Range(minTime * frameRate, maxTime * frameRate);
            float turningSpeed = Random.Range(40f / maxTime, 360f/maxTime);
            if (Random.Range(0f, 1f) > 0.5) {
                turnRate = new Vector3(0, turningSpeed, 0);
            } else {
                turnRate = new Vector3(0, -turningSpeed, 0);
            }
        }
    }

    // When collide with prey, delete the prey
    void OnCollisionEnter(Collision other) {
        // If collides with mate
        if (other.gameObject.tag == tag && (mate == other.gameObject || other.gameObject.GetComponent<FoxScript>().mate == this.gameObject)) {
            // Subtract hunger and look at each other
            hunger -= (int)(maxHunger * matingHungerThreshold);
            Vector3 targetVector = new Vector3(other.transform.position.x, 0.5f, other.transform.position.z);
            transform.LookAt(targetVector);

            // Wait and then make baby fox
            startWait(2);
            // TODO: add new baby fox and decide if the wait it needed

            // Look away from each other and continue moving
            transform.RotateAround(transform.position, transform.up, 180f);
            swapWalkAndTurn();

        // If collides with prey
        } else if (other.gameObject.tag == preyType) {
            hunger += (int)(other.gameObject.transform.localScale.x * maxHunger);
            if (hunger > maxHunger) {
                hunger = maxHunger;
            }
            Destroy(other.gameObject);
        }
    }

    // Time in seconds to wait
    void startWait(int time) {
        waitBool = true;
        waitTotalTime = time * frameRate;
    }
}

