using UnityEngine;

[RequireComponent(typeof(AudioSource))] 
public class RockTrigger : MonoBehaviour
{
    [Header("Сохранение")]
    [Tooltip("Уникальное имя для этого камня (Rock_1, Rock_Cave и т.д.)")]
    [SerializeField] private string rockID = "Rock_1";

    [Header("Настройки Камня")]
    [SerializeField] private GameObject rock; 
    [SerializeField] private float n = 30f;    // падение вниз
    [SerializeField] private float m = 10f;    // катание вправо
    [SerializeField] private float fallSpeed = 120f;
    [SerializeField] private float rollSpeed = 5f;
    [SerializeField] private float rotationSpeed = 300f;

    [Header("Фонарь")]
    [SerializeField] private GameObject FonarOnGround;
    [SerializeField] private GameObject FonarInHands;

    [Header("Звук")]
    [SerializeField] private AudioClip rockSound; 
    [SerializeField] private float soundDelay = 0.5f; 

    private Vector2 startPosition;
    private bool isFalling = false;
    private bool isRolling = false;
    private bool isActivated = false;
    private AudioSource audioSource; 

    private void Start()
    {
        if (this.rock != null)
        {
            this.startPosition = this.rock.transform.position;
        }

        // --- ПРОВЕРКА СОХРАНЕНИЯ ---
        // Если в памяти есть запись, что этот камень уже упал (1)
        if (GameSession.GetInt(rockID) == 1)
        {
            isActivated = true; // Блокируем повторную активацию

            // 1. Сразу телепортируем камень в конечную точку
            if (this.rock != null)
            {
                // Конечная точка: StartX + m, StartY - n
                Vector2 finalPos = new Vector2(this.startPosition.x + this.m, this.startPosition.y - this.n);
                this.rock.transform.position = finalPos;
                
                // Можно немного повернуть его, чтобы не стоял слишком ровно
                this.rock.transform.rotation = Quaternion.Euler(0, 0, -90f);
            }

            // 2. Настраиваем фонари (как будто сцена уже прошла)
            if (FonarOnGround != null) FonarOnGround.SetActive(true);
            if (FonarInHands != null) Destroy(FonarInHands);

            // Отключаем сам триггер, чтобы игрок не задел его снова
            GetComponent<Collider2D>().enabled = false;

            return; // Выходим, чтобы не настраивать звук
        }

        // Если сохранения нет, настраиваем звук как обычно
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; 
    }

    private void Update()
    {
        // Если уже активирован и сохранение загружено (камень внизу), логика движения не сработает,
        // так как isFalling и isRolling по умолчанию false
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
                // Не сбрасываем isActivated в false, чтобы Update не работал зря, 
                // но и флаги isFalling/isRolling уже false, так что движение остановится.
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == true && this.isActivated == false)
        {
            this.isActivated = true;
            
            // --- СОХРАНЯЕМ ФАКТ ПАДЕНИЯ ---
            GameSession.SetInt(rockID, 1);

            this.isFalling = true;

            if (FonarInHands != null) Destroy(FonarInHands); 
            if (FonarOnGround != null) FonarOnGround.SetActive(true);

            if (rockSound != null && audioSource != null)
            {
                audioSource.clip = rockSound;
                audioSource.PlayDelayed(soundDelay); 
            }
            
            // Отключаем триггер, чтобы не сработал дважды
            GetComponent<Collider2D>().enabled = false;
        }
    }
}