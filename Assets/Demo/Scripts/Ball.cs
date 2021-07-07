using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Ball : MonoBehaviour
{
    public event Action HitPlatformEvent;
    public event Action HitGoalEvent;
    public event Action DropEvent;

    private const float c_DropThresh = -5; // world!
    private const string PlatformTag = "Platform";
    private const string GoalTag = "Goal";

    [SerializeField]
    private Vector3 m_ForceMultiplier = Vector3.forward * 25;
    private Rigidbody m_Rigidbody;

    public void Initialize()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        m_Rigidbody.isKinematic = true;
    }

    public void Throw(float normForce)
    {
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.AddForce(m_ForceMultiplier * (normForce + 1), ForceMode.Impulse);
    }

    private void Update()
    {
        if (transform.position.y < c_DropThresh)
        {
            DropEvent.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(PlatformTag))
        {
            HitPlatformEvent.Invoke();
        }
        else if (collision.collider.CompareTag(GoalTag))
        {
            HitGoalEvent.Invoke();
        }
    }
}
