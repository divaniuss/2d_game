using UnityEngine;
using System.Collections; 

public class fireball_shoot : MonoBehaviour
{
    [SerializeField] private bool hasWeapon = false;
    [Header("Настройки Арбалета")]
    [SerializeField] private GameObject ArrowPrefab;
    [SerializeField] private GameObject PersonWhoShot;
    [SerializeField] private float ArrowSpeed = 30f;
    [SerializeField] private float SpawnDistance = 1f;
    // Добавляем переменную для контроля высоты
    [Tooltip("Коррекция высоты. Отрицательное значение опустит стрелу ниже.")]
    [SerializeField] private float SpawnHeightOffset = -0.2f; // <--- НОВОЕ ПОЛЕ
    [SerializeField] private float ShootDelay = 1.5f; 
    
    [Header("Визуал Арбалета")]
    [Tooltip("Объект арбалета СО стрелой")]
    [SerializeField] private GameObject CrossbowWithArrow;
    [Tooltip("Объект арбалета БЕЗ стрелы")] 
    [SerializeField] private GameObject CrossbowEmpty;

    [Header("Настройки Физики")]
    [Tooltip("0 = летит как лазер. 0.3 = легкая баллистика. 1 = падает как камень.")]
    [SerializeField] private float ArrowGravity = 0.1f; 

    [Header("Звуки")]
    [SerializeField] private AudioClip ShootSound;    
    [Range(0f, 3f)] [SerializeField] private float ShootVolume = 1f; 

    [SerializeField] private AudioClip ReloadSound;   
    [Range(0f, 3f)] [SerializeField] private float ReloadVolume = 1f; 
    
    [Tooltip("Через сколько секунд после выстрела начать проигрывать звук перезарядки")]
    [SerializeField] private float ReloadStartDelay = 0.5f; 

    [SerializeField] private AudioClip crossbowSound; 
    [Range(0f, 1f)] [SerializeField] private float crossbowVolume = 1f;

    private float shootTimer = 0f;

    void Start()
    {
        // При старте игры:
        // Если оружие уже есть (галочка в инспекторе) — показываем его.
        // Если оружия нет — прячем обе модели арбалета в руках.
        if (hasWeapon)
        {
            UpdateCrossbowVisuals(true);
        }
        else
        {
            if (CrossbowWithArrow != null) CrossbowWithArrow.SetActive(false);
            if (CrossbowEmpty != null) CrossbowEmpty.SetActive(false);
        }
    }

    void Update()
    {
        if (this.shootTimer > 0f)
        {
            this.shootTimer -= Time.deltaTime;
        }

        // --- ГЛАВНОЕ ИЗМЕНЕНИЕ ---
        // Добавили проверку "&& hasWeapon". 
        // Если hasWeapon == false, код внутри не сработает, выстрела не будет.
        if (Input.GetKeyDown(KeyCode.F) && this.shootTimer <= 0f && hasWeapon)
        {
            ShootArrow();
            
            this.shootTimer = this.ShootDelay;
            StartCoroutine(ReloadVisualsRoutine());

            if (ReloadSound != null)
            {
                StartCoroutine(PlayReloadSound());
            }
        }
    }

    // --- НОВЫЙ МЕТОД: ВЫЗЫВАЕТСЯ, КОГДА ПОДБИРАЕМ АРБАЛЕТ ---
    public void PickUpCrossbow()
    {

        if (crossbowSound != null)
        {
            AudioSource.PlayClipAtPoint(crossbowSound, transform.position, crossbowVolume);
        }
        hasWeapon = true; // Разрешаем стрельбу
        UpdateCrossbowVisuals(true); // Показываем арбалет в руках
    }

    void ShootArrow()
    {
        // 1. ЗВУК
        if (this.ShootSound != null)
        {
            PlaySoundCustom(this.ShootSound, this.PersonWhoShot.transform.position, this.ShootVolume);
        }

        // 2. МАТЕМАТИКА
        Vector2 direction = this.PersonWhoShot.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        
        // Рассчитываем базовую позицию (центр + дистанция вперед)
        Vector3 basePosition = this.PersonWhoShot.transform.position + (Vector3)direction * this.SpawnDistance;
        
        // <--- ИЗМЕНЕНО: Добавляем корректировку по высоте (по оси Y)
        Vector3 spawnPosition = new Vector3(basePosition.x, basePosition.y + SpawnHeightOffset, basePosition.z);

        // 3. СОЗДАНИЕ СТРЕЛЫ
        GameObject newArrow = Instantiate(this.ArrowPrefab, spawnPosition, Quaternion.identity);

        // 4. ВИЗУАЛ (Поворот стрелы)
        if (direction == Vector2.left)
        {
            Vector3 localScale = newArrow.transform.localScale;
            localScale.x *= -1f;
            newArrow.transform.localScale = localScale;
        }

        // 5. ФИЗИКА
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = this.ArrowGravity; 
            rb.linearVelocity = direction * this.ArrowSpeed;
        }

        Destroy(newArrow, 2f);
    }

    // --- ЛОГИКА СМЕНЫ МОДЕЛЕЙ ---
    IEnumerator ReloadVisualsRoutine()
    {
        UpdateCrossbowVisuals(false);
        yield return new WaitForSeconds(ShootDelay);
        UpdateCrossbowVisuals(true);
    }

    void UpdateCrossbowVisuals(bool isLoaded)
    {
        if (CrossbowWithArrow != null) CrossbowWithArrow.SetActive(isLoaded);
        if (CrossbowEmpty != null) CrossbowEmpty.SetActive(!isLoaded);
    }
    // ----------------------------

    IEnumerator PlayReloadSound()
    {
        yield return new WaitForSeconds(ReloadStartDelay);

        GameObject audioObj = new GameObject("ReloadSound");
        audioObj.transform.position = this.PersonWhoShot.transform.position;
        audioObj.transform.parent = this.PersonWhoShot.transform; 

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = ReloadSound;
        source.volume = ReloadVolume; 
        source.spatialBlend = 0f; 
        
        source.Play();

        Destroy(audioObj, ReloadSound.length);
    }

    void PlaySoundCustom(AudioClip clip, Vector3 position, float volume)
    {
        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = position;
        
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume; 
        source.spatialBlend = 0f; 
        
        source.Play();
        Destroy(go, clip.length);
    }
}