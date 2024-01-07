using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalScript : MonoBehaviour
{
    // User decide / survivability
    public float FOV;
    public float viewDistance;
    public float speed;
    protected float maxHunger = 60;    // in speed per second
    protected float hunger;

    // Growing
    protected bool mature = false;
    protected float size = 0.25f;
    protected float sizePerSecond;
    protected float timeToMature = 10;
    
    // Hunting / Mating
    protected string preyTag;
    protected GameObject huntedPrey = null;
    protected GameObject mate = null;
    protected float matingHungerThreshold = 0.5f;

    // Waiting
    protected bool waitBool = false;
    protected float waitTotalTime;
    protected float waitCounter;

    // Movement
    protected int frameRate;
    protected int moveTime;
    protected int moveCounter;
    protected Vector3 turnRate;
    protected bool walk = true;

    // Coloring
    protected Color mateColor = Color.blue;       // cyan
    protected Color huntColor = Color.blue;       // magenta
    protected Color passiveColor = Color.blue;    // blue

    // Start is called before the first frame update
    void Start()
    {
        hunger = maxHunger;
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;

        GetComponent<Renderer>().material.color = passiveColor;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!waitBool) {
            // If not mature, grow
            if (!mature) {
                Grow();
            }

            // Handle hunger
            Hunger();
            
            // If in sight, look for prey
            huntedPrey = See(preyTag);

            // If in sight and mature, look for a mate
            if (mature) {
                mate = See(tag);
            }

            // Move towards mate then prey then randomly
            Move();
        } else {
            waitCounter++;
            if (waitCounter > waitTotalTime) {
                waitCounter = 0;
                waitBool = false;
            }
        }
    }

    // Grow and increase in size   
    protected void Grow() {
        size += sizePerSecond;
        transform.localScale = new Vector3(size, size, size);
        if (size >= 1) {
            mature = true;
        }
    }

    // Cost hunger, die if starve
    protected void Hunger() {
        // hunger decreases at speed per second
        hunger -= speed / frameRate;
        if (hunger < 0) {
            Destroy(gameObject);
        }
    }

    // Find the closest target and if in sight, set them as the target
    protected GameObject See(string searchTag) {
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
                
                // Based on what it sees
                switch (searchTag) {
                    // Prey
                    case var preyTag when preyTag == preyTag:
                        if (closer) {
                            finalTarget = target;
                            closestFinalTarget = distancetoPrey;
                        }
                        break;
                    
                    // Mate
                    case var tag when tag == tag:
                        bool canSelfMate = hunger > (maxHunger * matingHungerThreshold);
                        // System.Type type = GetType();
                        bool canTargetMate = target.GetComponent<AnimalScript>().hunger > (target.GetComponent<AnimalScript>().maxHunger * matingHungerThreshold);
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
    protected void Move() {
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

            // Move towards prey
            Vector3 targetVector = new Vector3(huntedPrey.transform.position.x, transform.position.y, huntedPrey.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);
        
        } else {
            GetComponent<Renderer>().material.color = passiveColor;

            // Once time expires, change walking and turning
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
    protected void swapWalkAndTurn() {
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
    protected void OnCollisionEnter(Collision other) {
        // If collides with mate
        // System.Type type = GetType();
        if (other.gameObject.tag == tag && (mate == other.gameObject || other.gameObject.GetComponent<AnimalScript>().mate == this.gameObject)) {
            // Subtract hunger and look at each other
            hunger -= (int)(maxHunger * (matingHungerThreshold / 2));
            Vector3 targetVector = new Vector3(other.transform.position.x, 0.5f, other.transform.position.z);
            transform.LookAt(targetVector);

            // Wait
            startWait(2);

            // If lower X position, be the one to spawn the baby
            if (transform.position.x <= other.gameObject.transform.position.x) {
                // float averageSpeed = (speed + getOpponentSpeed(other.gameObject).speed) / 2;
                float averageSpeed = (speed + other.gameObject.GetComponent<AnimalScript>().speed) / 2;
                if (Random.Range(0f, 1f) > 0.5) {
                    averageSpeed += 1;
                }

                GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().spawnRabbit(transform.position.x + 0.75f, transform.position.z + 0.75f, FOV, viewDistance, averageSpeed);
            }

            // Look away from each other and continue moving
            transform.RotateAround(transform.position, transform.up, 180f);
            swapWalkAndTurn();
        
        // If collides with prey
        } else if (other.gameObject.tag == preyTag) {
            hunger += (int)(other.gameObject.transform.localScale.x * maxHunger);
            if (hunger > maxHunger) {
                hunger = maxHunger;
            }
            Destroy(other.gameObject);
        }
    }

    // Time in seconds to wait
    protected void startWait(int time) {
        waitBool = true;
        waitTotalTime = time * frameRate;
    }

    /////////////// Functions all child classes will have to implement ///////////////
    
    // This will get the specific gameObject we are looking for 
    // protected abstract float getOpponentSpeed(gameObject opponent);
    // Should return searchedObject.getComponent<RabbitScript>();
}
