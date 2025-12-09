using UnityEngine;
using System.Collections;

public class new_camera_move : MonoBehaviour
{
    [Header("Цель")]
    [SerializeField] private Transform player; 

    [Header("Настройки")]
    [SerializeField] private float smoothTime = 0.2f; 
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 400f;

    [Header("Настройки Тряски")]
    [SerializeField] private float shakeMagnitude = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Transform originalTarget;
    private Vector3 shakeOffset = Vector3.zero; 
    
    // ЭТО ГЛАВНОЕ: Координаты "виртуальной" камеры, которая не трясется
    private Vector3 logicalPosition; 

    void Start()
    {
        if (player != null) originalTarget = player;
        
        // Запоминаем стартовую позицию
        logicalPosition = transform.position;
    }

    void LateUpdate()
    {
        if (player == null && originalTarget != null) player = originalTarget;
        if (player == null) return;

        // 1. 
        // ВАЖНО: Мы берем Y от logicalPosition, а не от transform.position. 
        // Это гарантирует, что тряска не собьет высоту камеры.
        Vector3 targetPosition = new Vector3(player.position.x, logicalPosition.y, transform.position.z);
        
        // Ограничиваем X
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        
        // 2. Плавно двигаем "призрака" (SmoothDamp обновляет переменную logicalPosition)
        logicalPosition = Vector3.SmoothDamp(logicalPosition, targetPosition, ref velocity, smoothTime);
        
        // 3. А теперь ставим реальную камеру на место призрака + тряска
        transform.position = logicalPosition + shakeOffset;
    }

    public void ChangeTarget(Transform newTarget)
    {
        player = newTarget;
        // Когда меняем цель, можно обновить logicalPosition, чтобы не было рывка
        // Но SmoothDamp и так справится
    }

    public void ShakeCamera(float duration = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration));
    }

    IEnumerator ShakeRoutine(float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            shakeOffset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}