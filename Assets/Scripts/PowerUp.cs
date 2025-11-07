using UnityEngine;

public enum PowerUpType
{
    None,
    Pushback,
    Rockets,
    Smash
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;  // Assign in Inspector (Pushback or Rockets)
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.hasPowerup = true;
                player.currentPowerUp = powerUpType;
            }

            Destroy(gameObject);
        }
    }
}
