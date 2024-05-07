using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMovementScript : MonoBehaviour
{
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float waypointTolerance = 1f;
    public float detectionDistance = 5f;
    public float chaseWaitTime = 2f;

    private Transform target;
    private int currentWaypoint = 0;
    private float chaseTimer;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < detectionDistance)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0)
            return;

        // Düşmanın hedef noktasına doğru hareket etmesini sağla
        Transform waypoint = waypoints[currentWaypoint];
        Vector3 direction = (waypoint.position - transform.position).normalized;
        transform.position += direction * patrolSpeed * Time.deltaTime;

        // Hedef noktaya yeterince yaklaşıldığında, bir sonraki noktaya geç
        if (Vector3.Distance(transform.position, waypoint.position) < waypointTolerance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void Chase()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        chaseTimer += Time.deltaTime;
        if (chaseTimer >= chaseWaitTime)
        {
            chaseTimer = 0f;
            currentWaypoint = 0; // Kovalamacadan sonra ilk waypoint'e dön
        }
    }
}
