using UnityEngine;

public class BossStartTrigger : MonoBehaviour
{
    [SerializeField] private dragon_logic bossScript; // Ссылка на босса
    [SerializeField] private bool destroyAfterTrigger = true; // Удалить триггер после активации?
    [SerializeField] private new_camera_move cameraScript; // Ссылка на скрипт камеры (висит на Main Camera)
    [SerializeField] private Transform arenaCenterPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что зашел именно игрок (проверь, что у игрока тег Player)
        if (collision.CompareTag("Player"))
        {
            if (bossScript != null) 
            {
                bossScript.WakeUpBoss(); // Будим дракона
            }
 
            if (cameraScript != null && arenaCenterPoint != null)
            {
                // Говорим камере следить за точкой в центре арены
                cameraScript.ChangeTarget(arenaCenterPoint);
            }

            if (destroyAfterTrigger)
            {
                Destroy(gameObject); // Удаляем этот невидимый триггер, чтобы не срабатывал дважды
            }
        }
    }
}