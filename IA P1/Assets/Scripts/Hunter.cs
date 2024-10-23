using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    public enum StateType { Rest, Patrol, Chase }
    public StateType currentStateType = StateType.Patrol;

    public float energy = 100f;
    public float maxEnergy = 100f;
    public float energyDrainRate = 1f;
    public float restDuration = 5f;
    public Transform[] waypoints;
    public float waypointThreshold = 1f;
    public float visionRadius = 10f;
    public float obstacleAvoidanceRadius = 5f;
    public LayerMask obstacleLayer;

    public int currentWaypointIndex = 0;
    public bool isReversing = false;
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public float maxForce = 20f;
    public float maxSpeed = 10f;

    private State currentState;

    void Start()
    {
        ChangeState(new PatrolState(this));
    }

    void Update()
    {
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

    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    public Vector3 Pursuit(GameObject target)
    {
        Vector3 targetPos = target.transform.position;
        Vector3 targetVelocity = target.GetComponent<Boid>().velocity;
        float predictionTime = Vector3.Distance(transform.position, targetPos) / maxSpeed;
        Vector3 predictedTargetPos = targetPos + targetVelocity * predictionTime;

        Vector3 desired = (predictedTargetPos - transform.position).normalized * maxSpeed;
        return desired;
    }

    public Vector3 AvoidObstacles(Vector3 direction)
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, obstacleAvoidanceRadius, obstacleLayer);
        foreach (Collider obstacle in obstacles)
        {
            Vector3 avoidDirection = transform.position - obstacle.transform.position;
            avoidDirection.Normalize();
            avoidDirection *= maxSpeed;
            direction += avoidDirection;
        }

        if (direction != Vector3.zero)
        {
            direction.Normalize();
            direction *= maxSpeed;
            direction -= velocity;
            direction = Vector3.ClampMagnitude(direction, maxForce);
            direction.y = 0; 
        }

        return direction;
    }

    public GameObject FindClosestBoid()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag("Boid");
        GameObject closestBoid = null;
        float closestDistance = visionRadius;

        foreach (GameObject boid in boids)
        {
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < closestDistance)
            {
                closestBoid = boid;
                closestDistance = distance;
            }
        }

        return closestBoid;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boid"))
        {
            other.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleAvoidanceRadius);
    }
}