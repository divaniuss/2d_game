using UnityEngine;
using System.Collections; // Нужно для корутин

public class new_camera_move : MonoBehaviour
{
    [Header("Цель")]
    [SerializeField] private Transform player; 

    [Header("Настройки")]
    [SerializeField] private float smoothTime = 0.2f; 
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 400f;

    [Header("Настройки Тряски")] // <--- НОВЫЙ РАЗДЕЛ
    [SerializeField] private float shakeMagnitude = 0.3f; // Сила тряски

    private Vector3 velocity = Vector3.zero;
    private Transform originalTarget;
    private Vector3 shakeOffset = Vector3.zero; // Смещение от тряски

    void Start()
    {
        if (player != null) originalTarget = player;
    }

    void LateUpdate()
    {
        if (player == null && originalTarget != null) player = originalTarget;
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        
        // 1. Считаем плавную позицию
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        
        // 2. Добавляем к ней тряску (shakeOffset)
        transform.position = smoothedPos + shakeOffset;
    }

    public void ChangeTarget(Transform newTarget)
    {
        player = newTarget; 
    }

    // --- НОВЫЙ ПУБЛИЧНЫЙ МЕТОД: ВЫЗВАТЬ ТРЯСКУ ---
    public void ShakeCamera(float duration = 0.2f)
    {
        StartCoroutine(ShakeRoutine(duration));
    }

    IEnumerator ShakeRoutine(float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Генерируем случайную точку внутри сферы радиусом shakeMagnitude
            // Используем только X и Y, Z не трогаем
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            shakeOffset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем камеру на место
        shakeOffset = Vector3.zero;
    }
}