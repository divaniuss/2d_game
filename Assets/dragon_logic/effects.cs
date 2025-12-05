using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    // Время жизни эффекта. Подстройте под длительность вашей анимации.
    [SerializeField] private float lifetime = 0.18f; 

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}