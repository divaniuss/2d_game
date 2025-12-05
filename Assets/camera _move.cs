using UnityEngine;

public class camera_move : MonoBehaviour
{
    public float moveDistanceX = 120f;
    public float moveDistanceY = 0f;
    public float moveSpeed = 3f;

    private bool moveCamera = false;
    private Vector3 targetPosition;
    private Transform cam;

    void Start()
    {
        this.cam = Camera.main.transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == true && this.moveCamera == false)
        {
            this.moveCamera = true;
            this.targetPosition = this.cam.position + new Vector3(this.moveDistanceX, this.moveDistanceY, 0);
        }
    }

    void Update()
    {
        if (this.moveCamera == true)
        {
            this.cam.position = Vector3.Lerp(this.cam.position, this.targetPosition, Time.deltaTime * this.moveSpeed);

            if (Vector3.Distance(this.cam.position, this.targetPosition) < 0.05f)
            {
                this.cam.position = this.targetPosition;
                this.moveCamera = false;
            }
        }
    }
}
