using Unity.VisualScripting;
using UnityEngine;

public class RockTrigger : MonoBehaviour
{
    [SerializeField] private GameObject rock; // сам валун
    [SerializeField] private float n = 30f;    // расстояние падения вниз
    [SerializeField] private float m = 10f;   // расстояние катания вправо
    [SerializeField] private float fallSpeed = 120f;
    [SerializeField] private float rollSpeed = 5f;
    [SerializeField] private float rotationSpeed = 300f;

    [SerializeField] private GameObject FonarOnGround;
    [SerializeField] private GameObject FonarInHands;

    private Vector2 startPosition;
    private bool isFalling = false;
    private bool isRolling = false;
    private bool isActivated = false;

    private void Start()
    {
        if (this.rock != null)
        {
            this.startPosition = this.rock.transform.position;
        }
    }

    private void Update()
    {
        if (this.isActivated == false || this.rock == null)
            return;

        if (this.isFalling == true)
        {
            this.rock.transform.position = Vector2.MoveTowards(
                this.rock.transform.position,
                new Vector2(this.startPosition.x, this.startPosition.y - this.n),
                this.fallSpeed * Time.deltaTime
            );

            if (Vector2.Distance(this.rock.transform.position, new Vector2(this.startPosition.x, this.startPosition.y - this.n)) < 0.01f)
            {
                this.isFalling = false;
                this.isRolling = true;
            }
        }

        if (this.isRolling == true)
        {
            this.rock.transform.position = Vector2.MoveTowards(
                this.rock.transform.position,
                new Vector2(this.startPosition.x + this.m, this.startPosition.y - this.n),
                this.rollSpeed * Time.deltaTime
            );

            this.rock.transform.Rotate(Vector3.forward, -this.rotationSpeed * Time.deltaTime);

            if (Vector2.Distance(this.rock.transform.position, new Vector2(this.startPosition.x + this.m, this.startPosition.y - this.n)) < 0.01f)
            {
                this.isRolling = false;
                this.isActivated = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == true && this.isActivated == false)
        {
            this.isActivated = true;
            this.isFalling = true;
            Destroy(FonarInHands);
            FonarOnGround.SetActive(true);

        }
    }
}
