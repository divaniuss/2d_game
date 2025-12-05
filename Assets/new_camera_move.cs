using UnityEngine;

public class new_camera_move : MonoBehaviour
{
    [Header("Цель")]
    [SerializeField] private Transform player; // Текущая цель (сначала Игрок, потом Арена)

    [Header("Настройки")]
    [SerializeField] private float smoothTime = 0.2f; 
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 400f;

    private Vector3 velocity = Vector3.zero;
    private Transform originalTarget; // игрока

    void Start()
    {
        if (player != null)
        {
            originalTarget = player;
        }
    }

    void LateUpdate()
    {
        // --- ЛОГИКА ВОЗВРАТА ---
        // Если текущая цель исчезла (босс удалил ArenaCenter), а игрок жив — переключаемся на игрока
        if (player == null && originalTarget != null)
        {
            player = originalTarget;
        }

        // Если совсем не за кем следить — выходим
        if (player == null) return;


        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        
        // Плавное движение
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    // Метод для триггера босса
    public void ChangeTarget(Transform newTarget)
    {
        player = newTarget; 
    }
}