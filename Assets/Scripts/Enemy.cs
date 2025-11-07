using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody enemyRb;
    private GameObject player;
    public float speed = 3.0f;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player"); 
    }

    void Update()
    {
        // Skip movement if the game is paused or hasn’t started yet
        if (GameManager.Instance != null && (!GameManager.Instance.gameStarted || GameManager.Instance.isPaused))
            return;

        if (player != null)
        {
            Vector3 lookDirection = (player.transform.position - transform.position);
            lookDirection.y = 0;
            enemyRb.AddForce(lookDirection.normalized * speed * Time.deltaTime, ForceMode.VelocityChange);
        }

        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Play enemy collision sound
            AudioManager.Instance?.PlayEnemyEnemyCollisionSFX();
        }
    }

}
