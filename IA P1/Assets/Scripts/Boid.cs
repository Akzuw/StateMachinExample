using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public enum StateType { Flocking, Waypoint, Evading, Arriving }
    public StateType currentStateType = StateType.Flocking;

    public Transform targetFood; 
    public Transform hunter; 
    public float maxForce = 20; 
    public float maxSpeed = 10; 
    public float visionRadius = 20; 
    public float arriveRadius = 0.5f; 
    public float separationRadius = 5; 
    public float alignmentRadius = 10; 
    public float cohesionRadius = 10;
    public float obstacleRadius = 2.0f; 
    public Vector3 velocity = Vector3.zero; 
    public LayerMask obstacleLayer; 
    public Vector3 acceleration = Vector3.zero; 
    public Transform centralWaypoint; 

    private BoidState currentState;

    void Start()
    {
        ChangeState(new FlockingState(this));
    }

    void Update()
    {
        
        FindClosestFood();

        
        if (hunter != null && Vector3.Distance(transform.position, hunter.position) < visionRadius)
        {
            if (!(currentState is EvadingState))
            {
                ChangeState(new EvadingState(this));
            }
        }
        
        else if (targetFood != null && Vector3.Distance(transform.position, targetFood.position) < visionRadius)
        {
            if (!(currentState is ArrivingState))
            {
                ChangeState(new ArrivingState(this));
            }
        }
        
        else if (currentStateType != StateType.Flocking)
        {
            ChangeState(new FlockingState(this));
        }
       
        else if (centralWaypoint != null && Vector3.Distance(transform.position, centralWaypoint.position) > visionRadius)
        {
            if (!(currentState is WaypointState))
            {
                ChangeState(new WaypointState(this));
            }
        }

        currentState.Execute();

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        velocity.y = 0; 
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
        acceleration = Vector3.zero; 
    }

    public void ChangeState(BoidState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();

        
        if (newState is FlockingState)
        {
            currentStateType = StateType.Flocking;
        }
        else if (newState is WaypointState)
        {
            currentStateType = StateType.Waypoint;
        }
        else if (newState is EvadingState)
        {
            currentStateType = StateType.Evading;
        }
        else if (newState is ArrivingState)
        {
            currentStateType = StateType.Arriving;
        }
    }

    public void FindClosestFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        GameObject closestFood = null;
        float closestDistance = visionRadius;

        foreach (GameObject food in foods)
        {
            float distance = Vector3.Distance(transform.position, food.transform.position);
            if (distance < closestDistance)
            {
                closestFood = food;
                closestDistance = distance;
            }
        }

        if (closestFood != null)
        {
            targetFood = closestFood.transform;
        }
        else
        {
            targetFood = null;
        }
    }

    public Vector3 Separate()
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

        foreach (Boid other in FindObjectsOfType<Boid>())
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < separationRadius)
            {
                Vector3 diff = transform.position - other.transform.position;
                diff.Normalize();
                diff /= distance; 
                steer += diff;
                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
        }

        if (steer.magnitude > 0)
        {
            steer.Normalize();
            steer *= maxSpeed;
            steer -= velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);
            steer.y = 0; 
        }

        return steer;
    }

    public Vector3 Align()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Boid other in FindObjectsOfType<Boid>())
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < alignmentRadius)
            {
                sum += other.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= maxSpeed;
            Vector3 steer = sum - velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);
            steer.y = 0; 
            return steer;
        }

        return Vector3.zero;
    }

    public Vector3 Cohere()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Boid other in FindObjectsOfType<Boid>())
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < cohesionRadius)
            {
                sum += other.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            return Seek(sum);
        }

        return Vector3.zero;
    }

    public Vector3 AvoidObstacles()
    {
        Vector3 steer = Vector3.zero;
        bool obstacleDetected = false;

        if (Physics.Raycast(transform.position + transform.right * 0.25f, transform.right, out RaycastHit hitRight, obstacleRadius, obstacleLayer))
        {
            steer += Seek(transform.position - transform.right);
            obstacleDetected = true;
        }

        if (Physics.Raycast(transform.position - transform.right * 0.25f, -transform.right, out RaycastHit hitLeft, obstacleRadius, obstacleLayer))
        {
            steer += Seek(transform.position + transform.right);
            obstacleDetected = true;
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitForward, obstacleRadius, obstacleLayer))
        {
            steer += Seek(transform.position - transform.forward);
            obstacleDetected = true;
        }

        if (obstacleDetected)
        {
            steer.Normalize();
            steer *= maxSpeed;
            steer -= velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);
            steer.y = 0; 
        }

        return steer;
    }

    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desired = targetPos - transform.position;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxForce);
        steer.y = 0; 

        return steer;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hunter"))
        {
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, obstacleRadius);
    }
}