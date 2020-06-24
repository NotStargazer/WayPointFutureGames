using UnityEngine;

[RequireComponent(typeof(Waypoints))]
public class MovingActor : MonoBehaviour
{
    public float m_Speed;
    public float m_RotationSpeed;

    int m_Index = 0;

    Waypoints m_Points;

    void Awake()
    {
        m_Points = GetComponent<Waypoints>();
    }

    void Update()
    {
        Vector3 target = new Vector3(m_Points.m_Waypoints[m_Index].x, transform.position.y, m_Points.m_Waypoints[m_Index].z);

        if (Vector3.Distance(transform.position, target ) < 1)
        {
            m_Index++;
            if (m_Index == m_Points.m_Waypoints.Count)
            {
                m_Index = 0;
            }
        }
        var newDir = Vector3.RotateTowards(transform.forward, target - transform.position, m_RotationSpeed * Time.deltaTime, 360);

        Debug.DrawRay(transform.position, newDir, Color.red);

        transform.rotation = Quaternion.LookRotation(newDir);

        transform.Translate(transform.forward * m_Speed * Time.deltaTime, Space.World);
    }
}
