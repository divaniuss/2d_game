using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class dragon_logic : MonoBehaviour
{
    [Header("Размер Босса")]
    [Tooltip("1 = стандарт, 1.5 = больше, 2 = огромный)")]
    [SerializeField] private float bodyScale = 1.5f;

    [Header("Характеристики")]
    [SerializeField] private int HealthPoints = 20;
    
    [Header("Настройки Старта Битвы")] // --- НОВЫЙ РАЗДЕЛ ---
    [Tooltip("Триггер (зона), куда босс должен прыгнуть при появлении")]
    [SerializeField] private BossZoneTrigger startingZone; 
    [SerializeField] private float entryJumpHeight = 3f;   

    [Header("Коррекция Позиции")]
    [Tooltip("Если дракон проваливается, увеличь это число (0.5, 1.0). Если летит - уменьши.")]
    [SerializeField] private float surfaceOffset = 0f; 

    [Header("Стрельба")]
    [SerializeField] private GameObject FireballPrefab; 
    [SerializeField] private Transform firePoint; 
    [SerializeField] private float shootInterval = 3f;  
    [SerializeField] private float fireballSpeed = 10f;
    [SerializeField] private float shootAnimDelay = 0.31f;

    [Header("Звуки и Громкость")]
    [SerializeField] private AudioClip AttackSound; 
    [Range(0f, 1f)] [SerializeField] private float attackVolume = 1f; 
    
    [SerializeField] private AudioClip JumpSound;   
    [Range(0f, 1f)] [SerializeField] private float jumpVolume = 1f;

    [SerializeField] private AudioClip FlySound;    
    [Range(0f, 1f)] [SerializeField] private float flyVolume = 1f;

    [SerializeField] private AudioClip HurtSound;   
    [Range(0f, 1f)] [SerializeField] private float hurtVolume = 1f;

    [SerializeField] private AudioClip DieSound;    
    [Range(0f, 1f)] [SerializeField] private float dieVolume = 1f;

    [SerializeField] private AudioClip StepSound;   
    [Range(0f, 1f)] [SerializeField] private float stepVolume = 0.5f; 

    [SerializeField] private AudioClip LandSound;   
    [Range(0f, 1f)] [SerializeField] private float landVolume = 0.7f; 

    [Header("Движение")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f; 
    [SerializeField] private float reactionDelay = 0.5f; 
    
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private float fallSpeed = 10f; 

    [Header("Эффекты")] 
    [SerializeField] private GameObject landingEffectPrefab;
    [SerializeField] private float landingEffectOffsetY = 1.0f;

    [SerializeField] private GameObject arena_stena; 
    [SerializeField] private GameObject arena_center;

    // Добавили состояние WaitingToStart
    private enum BossState { WaitingToStart, Idle, Preparing, Moving, Landing, Cooldown }
    private BossState currentState = BossState.WaitingToStart;

    private Animator anim;
    private AudioSource audioSource; 
    private BoxCollider2D col; 
    private bool isDead = false;
    private bool isRageMode = false; 
    private Transform playerTransform; 
    private float shootTimer;

    private float stepTimer = 0f;
    private float stepInterval = 0.4f; 

    private ZoneType currentZone = ZoneType.Ground; 
    private BossZoneTrigger targetTrigger = null; 

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
        col = GetComponent<BoxCollider2D>(); 
        shootTimer = shootInterval;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        // Принудительно ждем старта
        currentState = BossState.WaitingToStart;
    }

    void Update()
    {
        if (isDead) return;
        if (playerTransform == null) return; 

        // --- БЛОКИРОВКА: Если ждем старта, ничего не делаем ---
        if (currentState == BossState.WaitingToStart) return; 

        // 1. ЛОГИКА СМЕНЫ ЗОН
        if ((currentState == BossState.Idle || currentState == BossState.Cooldown) && targetTrigger != null)
        {
            if (currentZone != targetTrigger.zoneType)
            {
                StartCoroutine(MoveRoutine(targetTrigger));
            }
        }

        // 2. ПОВОРОТ
        if (currentState != BossState.Moving && currentState != BossState.Landing)
        {
            if (playerTransform.position.x > transform.position.x)
                transform.localScale = new Vector3(bodyScale, bodyScale, 1); 
            else 
                transform.localScale = new Vector3(-bodyScale, bodyScale, 1);
        }

        // 3. СТРЕЛЬБА
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            if (currentState != BossState.Idle && currentState != BossState.Cooldown)
            {
                ShootFireball(); 
            }
            else
            {
                StartCoroutine(AttackRoutine());
            }
            shootTimer = shootInterval;
        }

        // 4. ХОДЬБА
        if (currentState == BossState.Idle && currentZone == ZoneType.Ground && targetTrigger != null && targetTrigger.zoneType == ZoneType.Ground)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            
            // Идет вплотную
            if (dist > 0.1f) 
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), moveSpeed * Time.deltaTime);
                anim.SetBool("IsWalking", true); 

                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0)
                {
                    PlaySound(StepSound, stepVolume);
                    stepTimer = stepInterval;
                }
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
        }
        else if (currentState != BossState.Idle)
        {
             anim.SetBool("IsWalking", false);
        }
    }

    // --- НОВЫЙ МЕТОД: ВЫЗЫВАЕТСЯ ТРИГГЕРОМ ENTRY ---
    public void WakeUpBoss()
    {
        if (currentState == BossState.WaitingToStart)
        {
            if (startingZone != null)
            {
                StartCoroutine(EnterArenaRoutine());
            }
            else
            {
                Debug.LogError("Не назначен Starting Zone в инспекторе босса!");
                currentState = BossState.Idle; // Аварийный запуск
            }
        }
    }

    // --- КОРАТИНА ПОЯВЛЕНИЯ БОССА ---
    IEnumerator EnterArenaRoutine()
    {
        currentState = BossState.Moving;
        
        anim.SetTrigger("Jump");
        PlaySound(JumpSound, jumpVolume);

        Vector3 startPos = transform.position;
        Vector3 endPos = startingZone.standPoint.position;

        // Прыжок появления
        yield return StartCoroutine(MoveParabola(startPos, endPos, 1.5f, entryJumpHeight));

        currentState = BossState.Landing;
        transform.position = new Vector3(endPos.x, transform.position.y, endPos.z);
        
        yield return StartCoroutine(FallToGroundSafely(startingZone.standPoint.position.y));
        
        PlaySound(LandSound != null ? LandSound : StepSound, landVolume);

        // Взрыв при появлении
        if (landingEffectPrefab != null)
        {
             Vector3 effectPos = transform.position + Vector3.up * landingEffectOffsetY;
             Instantiate(landingEffectPrefab, effectPos, Quaternion.identity);
        }

        currentZone = startingZone.zoneType;
        targetTrigger = startingZone; 
        
        anim.SetTrigger("Idle");
        currentState = BossState.Idle; // Начинаем бой
    }

    public void UpdatePlayerPosition(BossZoneTrigger newTrigger)
    {
        targetTrigger = newTrigger;
    }

    IEnumerator MoveRoutine(BossZoneTrigger target)
    {
        currentState = BossState.Preparing; 
        anim.SetBool("IsWalking", false);
        yield return new WaitForSeconds(reactionDelay);

        currentState = BossState.Moving;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = target.standPoint.position; 

        bool shouldUseJump = ShouldJump(currentZone, target.zoneType);

        if (shouldUseJump)
        {
            anim.SetTrigger("Jump");
            PlaySound(JumpSound, jumpVolume);
            yield return StartCoroutine(MoveParabola(startPos, endPos, 1.0f, jumpHeight));
        }
        else
        {
            anim.SetTrigger("Fly"); 
            PlaySound(FlySound, flyVolume);
            yield return StartCoroutine(MoveLinear(startPos, endPos, 1.5f)); 
        }

        currentState = BossState.Landing;
        transform.position = new Vector3(endPos.x, transform.position.y, endPos.z);
        
        yield return StartCoroutine(FallToGroundSafely(target.standPoint.position.y));
        
        PlaySound(LandSound != null ? LandSound : StepSound, landVolume);

        // Взрыв при обычном перемещении
        if (landingEffectPrefab != null)
        {
            Vector3 effectPos = transform.position + Vector3.up * landingEffectOffsetY;
            Instantiate(landingEffectPrefab, effectPos, Quaternion.identity);
        }

        currentZone = target.zoneType;
        anim.SetTrigger("Idle"); 
        
        currentState = BossState.Cooldown;

        if (targetTrigger != null && currentZone != targetTrigger.zoneType)
            yield return new WaitForSeconds(0.5f); 
        else
            yield return new WaitForSeconds(3f); 

        currentState = BossState.Idle;
    }

    IEnumerator FallToGroundSafely(float safeYLevel)
    {
        float distToFeet = transform.position.y - col.bounds.min.y;
        float finalOffset = distToFeet + surfaceOffset;

        bool landed = false;
        float timeout = 2.0f;

        while (!landed && timeout > 0)
        {
            timeout -= Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            Vector2 boxOrigin = (Vector2)transform.position + Vector2.up * 1.0f; 
            Vector2 boxSize = new Vector2(col.size.x * 0.5f, 0.1f);
            
            RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, 1.0f + finalOffset + 0.1f, groundLayer);

            if (hit.collider != null)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + finalOffset, transform.position.z);
                landed = true;
            }
            
            if (transform.position.y < safeYLevel - 2.0f)
            {
                transform.position = new Vector3(transform.position.x, safeYLevel, transform.position.z);
                landed = true;
            }

            yield return null;
        }
        
        if (!landed)
        {
            transform.position = new Vector3(transform.position.x, safeYLevel, transform.position.z);
        }
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip, volume);
    }

    bool ShouldJump(ZoneType from, ZoneType to)
    {
        if (from == ZoneType.Ground || to == ZoneType.Ground) return true;
        if ((from == ZoneType.Green && to == ZoneType.Blue) || (from == ZoneType.Blue && to == ZoneType.Green)) return true;
        if ((from == ZoneType.Purple && to == ZoneType.Red) || (from == ZoneType.Red && to == ZoneType.Purple)) return true;
        return false;
    }

    IEnumerator MoveParabola(Vector3 start, Vector3 end, float duration, float height)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / duration;
            Vector3 linearPos = Vector3.Lerp(start, end, time);
            float yOffset = height * 4 * time * (1 - time);
            transform.position = linearPos + new Vector3(0, yOffset, 0);
            yield return null;
        }
    }

    IEnumerator MoveLinear(Vector3 start, Vector3 end, float duration)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, end, time);
            yield return null;
        }
    }

    IEnumerator AttackRoutine()
    {
        if (currentState != BossState.Moving && currentState != BossState.Landing)
        {
            if (anim != null) anim.SetTrigger("attack");
            yield return new WaitForSeconds(shootAnimDelay);
            ShootFireball();
        }
    }

    void ShootFireball()
    {
        if (playerTransform == null) return;

        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;

        // Целимся в грудь (поднимаем цель на 0.75f)
        Vector3 targetPos = playerTransform.position + Vector3.up * 0.75f; 

        Vector2 direction = (targetPos - spawnPos).normalized;

        GameObject newFireball = Instantiate(FireballPrefab, spawnPos, Quaternion.identity);

        Collider2D dragonCol = GetComponent<Collider2D>();
        Collider2D ballCol = newFireball.GetComponent<Collider2D>();
        if (dragonCol != null && ballCol != null) Physics2D.IgnoreCollision(dragonCol, ballCol);

        Rigidbody2D rb = newFireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f; 
            rb.linearVelocity = direction * fireballSpeed;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            newFireball.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (AttackSound != null)
        {
            AudioSource audio = newFireball.AddComponent<AudioSource>();
            audio.clip = AttackSound;
            audio.spatialBlend = 0f; 
            audio.volume = attackVolume; 
            audio.Play();
        }
        Destroy(newFireball, 3f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("strela"))
        {
            HealthPoints -= 1;
            Destroy(collision.gameObject);
            
            if (anim != null) anim.SetTrigger("Hurt");
            PlaySound(HurtSound, hurtVolume); 

            if (HealthPoints <= 5 && !isRageMode)
            {
                isRageMode = true;
                shootInterval /= 2f; 
                GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f); 
            }

            if (HealthPoints <= 0) Die();
        }
    }

    void Die()
    {
        isDead = true;
        PlaySound(DieSound, dieVolume); 
        if (anim != null) anim.SetTrigger("Die");
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject, 1.5f);
        Destroy(arena_stena);
        Destroy(arena_center);
    }
}