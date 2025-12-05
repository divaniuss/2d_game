using UnityEngine;

public class CoinFloat : MonoBehaviour
{
    private float startY;
    private float amplitude = 0.2f;
    private float frequency = 2f;

    void Start()
    {
        this.startY = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            this.startY + Mathf.Sin(Time.time * frequency) * amplitude,
            transform.position.z
        );
    }
}