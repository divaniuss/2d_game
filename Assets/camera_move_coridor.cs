using UnityEngine;

public class CameraMoveCoridor : MonoBehaviour
{
    [SerializeField] private float n = 100f;  // расстояние движения вправо
    [SerializeField] private float m = 12f;   // скорость движения
    [SerializeField] private float stopThreshold = 0.05f;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private Transform cam;

    void Start()
    {
        this.cam = Camera.main.transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == true && this.isMoving == false)
        {
            this.isMoving = true;
            this.targetPosition = this.cam.position + new Vector3(this.n, 0f, 0f);
        }
    }

    void Update()
    {
        if (this.isMoving == true)
        {
            this.cam.position = Vector3.MoveTowards(
                this.cam.position,
                this.targetPosition,
                this.m * Time.deltaTime
            );

            if (Vector3.Distance(this.cam.position, this.targetPosition) < this.stopThreshold)
            {
                this.cam.position = this.targetPosition;
                this.isMoving = false;
            }
        }
    }
}
