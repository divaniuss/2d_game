using UnityEngine;

public class shiipi_damage : MonoBehaviour
{
    [SerializeField] private int damage = 3;

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_health playerHealth = collision.GetComponent<Player_health>();
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
            }
        }
    }
}
