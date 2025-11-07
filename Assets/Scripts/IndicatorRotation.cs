using UnityEngine;

public class IndicatorRotation : MonoBehaviour
{
    public float speed = 100.0f;
    public MeshRenderer Renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * 5.7f;
        Material material = Renderer.material;
        
        material.color = new Color(0.15f, 1.20f, 50.3f, 10.4f);
        InvokeRepeating("ChangingColor", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime*speed);
    }

     void ChangingColor()
    {
        Material material = Renderer.material;
        material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 2f);
    }
}
