using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class ActivateLinkByDistance : MonoBehaviour
{
    public Transform enemy;
    public float activateDistance = 6f;

    private NavMeshLink navLink;

    void Start()
    {
        navLink = GetComponent<NavMeshLink>();
        navLink.activated = false;
    }

    void Update()
    {
        if (enemy == null || navLink == null) return;

        Vector3 startWorld = transform.TransformPoint(navLink.startPoint);
        Vector3 endWorld = transform.TransformPoint(navLink.endPoint);

        float distToStart = Vector3.Distance(enemy.position, startWorld);
        float distToEnd = Vector3.Distance(enemy.position, endWorld);

        bool shouldActivate = distToStart <= activateDistance || distToEnd <= activateDistance;

        if (navLink.activated != shouldActivate)
        {
            navLink.activated = shouldActivate;

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }
    }

    void OnDrawGizmos()
    {
        if (navLink == null) navLink = GetComponent<NavMeshLink>();
        if (navLink == null) return;

        Vector3 startWorld = transform.TransformPoint(navLink.startPoint);
        Vector3 endWorld = transform.TransformPoint(navLink.endPoint);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startWorld, 0.2f);
        Gizmos.DrawSphere(endWorld, 0.2f);
    }
}