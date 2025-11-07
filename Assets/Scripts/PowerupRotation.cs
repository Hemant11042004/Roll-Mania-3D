using UnityEngine;

public class PowerupRotation : MonoBehaviour
{
     public float speed = 50.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime*speed);
    }
}
