using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private float powerUpStrength = 15.0f;
    public float speed = 5.0f;
    public bool hasPowerup = false;
    public GameObject powerupIndicator;
    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing = false;
    float floorY;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        // Launch rockets if rocket powerup is active and F key pressed
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
            AudioManager.Instance?.PlayPowerupUseSFX(PowerUpType.Rockets);
        }

        if(currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) &&!smashing)
        {
        smashing = true;
        StartCoroutine(Smash());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerupIndicator.gameObject.SetActive(true);
            AudioManager.Instance?.PlayPowerupPickupSFX(currentPowerUp);
            Destroy(other.gameObject);
            

            // restart powerup timer if already active
            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());

            
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        currentPowerUp = PowerUpType.None;
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp ==
            PowerUpType.Pushback)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position -
            transform.position;
            enemyRigidbody.AddForce(awayFromPlayer * powerUpStrength,
            ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
            AudioManager.Instance?.PlayPowerupUseSFX(PowerUpType.Pushback);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            //  Play collision sound
            AudioManager.Instance?.PlayPlayerEnemyCollisionSFX();
        }
    }

    IEnumerator Smash()
    {
        
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        //Store the y position before taking off
        floorY = transform.position.y;
        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while(Time.time < jumpTime)
        {
        //move the player up while still keeping their x velocity.
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, smashSpeed);
        yield return null;
        }
        //Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, -smashSpeed * 2);
            yield return null;
        }
        AudioManager.Instance?.PlayPowerupUseSFX(PowerUpType.Smash);
        
        //Cycle through all enemies.
        for (int i = 0; i < enemies.Length; i++)
        {
        //Apply an explosion force that originates from our position.
        if(enemies[i] != null)
        enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce,
        transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
        //We are no longer smashing, so set the boolean to false
        smashing = false;
    }


    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
           tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up,
           Quaternion.identity);
           tmpRocket.GetComponent<RocketBehavior>().Fire(enemy.transform);
        }
    }

}
