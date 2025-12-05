using UnityEngine;
using UnityStandardAssets._2D; 

public class Enemy_damage : MonoBehaviour
{
    [Header("Настройки силы")]
    [SerializeField] private float knockbackForce = 12f; 
    [SerializeField] private float upwardForce = 8f;
    [SerializeField] private int damage = 1; // Урон врага

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Наносим урон
            Player_health playerHealth = collision.GetComponent<Player_health>();
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
            }

            // 2. Делаем отбрасывание
            PlatformerCharacter2D playerMove = collision.GetComponent<PlatformerCharacter2D>();
            if (playerMove != null)
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                
                Vector2 force = new Vector2(Mathf.Sign(direction.x) * knockbackForce, upwardForce);
                
                playerMove.ApplyKnockback(force);
            }
        }
    }
}