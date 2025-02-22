using UnityEngine;
using UnityEngine.AI;

// Walk to a random position and repeat
[RequireComponent(typeof(NavMeshAgent))]
public class RandomFly : MonoBehaviour
{
    public float m_Range = 25.0f;
    NavMeshAgent m_agent;

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    void Update() 
    {
        if (!m_agent.isOnNavMesh || m_agent.pathPending || m_agent.remainingDistance > 0.1f)
            return;

        var xzPlane = m_Range * Random.insideUnitCircle;
        m_agent.destination = transform.position + new Vector3(xzPlane.x, transform.position.y, xzPlane.y);
    }
}
