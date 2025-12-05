using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

[RequireComponent(typeof(AudioSource))]
public class Player_health : MonoBehaviour
{
    [SerializeField] private int HealthPoints = 3;
    
    [Header("UI Ссылки")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Звуки")]
    [SerializeField] private AudioClip hurtSound; // Звук ранения
    [SerializeField] private AudioClip dieSound;  // Звук смерти

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ApplyDamage(int damageAmount)
    {
        HealthPoints -= damageAmount;

 
        if (hurtSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (HealthPoints <= 0)
        {
            Die();
        }
        else
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashRed());
            }
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;       
        yield return new WaitForSeconds(0.2f); 
        spriteRenderer.color = Color.white;     
    }

    private void Die()
    {
        // --- 1. ЗВУК СМЕРТИ (НОВОЕ) ---
        if (dieSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dieSound);
        }

        // --- 2. Отключаем управление ---
        Platformer2DUserControl input = GetComponent<Platformer2DUserControl>();
        if (input != null) input.enabled = false; 

        PlatformerCharacter2D movement = GetComponent<PlatformerCharacter2D>();
        if (movement != null) movement.enabled = false;

        // --- 3. Блокируем физику ---
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // --- 4. Визуал смерти ---
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.black; 
        }

        // Удаляем игрока через полсекунды
        Destroy(gameObject, 0.5f);
        
        // --- 5. ВКЛЮЧАЕМ ЭКРАН СМЕРТИ ---
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, есть ли у объекта тег "fireball"
        if (collision.gameObject.CompareTag("fireball"))
        {
            ApplyDamage(1); // Наносим 1 единицу урона
            Destroy(collision.gameObject); // Уничтожаем сам фаербол, чтобы он не пролетел сквозь
        }
    }
}