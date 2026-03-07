using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent agent;
    private bool isJumping;

    private int jumpArea;
    private int normalMask;
    private int withJumpMask;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        agent.autoBraking = false;
        agent.stoppingDistance = 0.3f;

        jumpArea = NavMesh.GetAreaFromName("Jump");
        int jumpBit = 1 << jumpArea;

        withJumpMask = NavMesh.AllAreas;
        normalMask = NavMesh.AllAreas & ~jumpBit;

        agent.areaMask = normalMask;
    }

    void Update()
    {
        if (player is not null) return;
        if (!agent.isOnNavMesh) return;
        if (isJumping) return;

        NavMeshPath path = new NavMeshPath();

        bool foundNormalPath = NavMesh.CalculatePath(
            transform.position,
            player.position,
            normalMask,
            path
        );

        if (foundNormalPath && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.areaMask = normalMask;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.areaMask = withJumpMask;
            agent.SetDestination(player.position);
        }

        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(JumpAcrossLink());
        }
    }

    IEnumerator JumpAcrossLink()
    {
        isJumping = true;
        agent.isStopped = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        float duration = 0.45f;
        float jumpHeight = 1f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += jumpHeight * 4f * (t - t * t);
            transform.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        agent.CompleteOffMeshLink();
        agent.isStopped = false;
        isJumping = false;
    }
}