using UnityEngine;

public class arbalet_pick : MonoBehaviour
{
 void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что в арбалет врезался именно Игрок
        if (collision.CompareTag("Player"))
        {
            // Пытаемся найти на игроке скрипт стрельбы
            fireball_shoot shootingScript = collision.GetComponent<fireball_shoot>();

            if (shootingScript != null)
            {
                // 1. Включаем оружие игроку
                shootingScript.PickUpCrossbow();

                // 2. Уничтожаем этот объект на земле (или выключаем его)
                Destroy(gameObject); 
                // Если хочешь просто скрыть, используй: gameObject.SetActive(false);
            }
        }
    }
}
