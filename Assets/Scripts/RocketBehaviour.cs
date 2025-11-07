using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private Transform target;
    private float speed = 15.0f;
    private bool homing;
    private float rocketStrength = 15.0f;
    private float aliveTimer = 5.0f;

    void Update()
    {
        if (homing)
        {
            // If target is valid, keep chasing
            if (target != null)
            {
                Vector3 moveDirection = (target.position - transform.position).normalized;
                transform.position += moveDirection * speed * Time.deltaTime;
                transform.LookAt(target);
            }
            else
            {
                // Target is gone — self-destruct early (or fly straight)
                transform.Translate(Vector3.forward * speed * Time.deltaTime); // destroy after 1 second
            }
        }
        
    }

    public void Fire(Transform homingTarget)
    {
        target = homingTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }

    void OnCollisionEnter(Collision col)
    {
        if (target != null)
        {
            if (col.gameObject.CompareTag(target.tag))
            {
                Rigidbody targetRigidbody = col.gameObject.GetComponent<Rigidbody>();
                Vector3 away = -col.contacts[0].normal;
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);
                Destroy(gameObject);
            }
        }
        else
        {
            // If target somehow null but collision happens, destroy anyway
            Destroy(gameObject);
        }
    }
}
