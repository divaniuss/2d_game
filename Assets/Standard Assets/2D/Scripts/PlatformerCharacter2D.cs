using System;
using System.Collections;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(AudioSource))] // Автоматически добавит AudioSource
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // Максимальная скорость
        [SerializeField] private float m_JumpForce = 400f;                  // Сила прыжка
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Скорость в приседе
        [SerializeField] private bool m_AirControl = false;                 // Управление в воздухе
        [SerializeField] private LayerMask m_WhatIsGround;                  // Слои, считающиеся землей

        [Header("Звуки шагов")] // --- НОВЫЙ РАЗДЕЛ ---
        [SerializeField] private AudioClip[] m_FootstepSounds; // Сюда перетащить звуки шагов
        [SerializeField] private float m_StepInterval = 0.4f; // Как часто играть звук (подобрать под анимацию)

        private Transform m_GroundCheck;    
        const float k_GroundedRadius = .2f; 
        private bool m_Grounded;            
        private Transform m_CeilingCheck;   
        const float k_CeilingRadius = .01f; 
        private Animator m_Anim;            
        private Rigidbody2D m_Rigidbody2D;
        private AudioSource m_AudioSource; // --- Ссылка на аудио ---
        private bool m_FacingRight = true;  

        private bool m_IsStunned = false; 
        private float m_StepCycle = 0f; // --- Таймер для шагов ---

        private void Awake()
        {
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_AudioSource = GetComponent<AudioSource>(); // Получаем компонент
        }

        private void FixedUpdate()
        {
            m_Grounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Unity 6 linearVelocity
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.linearVelocity.y);
        }

        public void ApplyKnockback(Vector2 force)
        {
            if (m_IsStunned) return;

            m_IsStunned = true; 
            m_Rigidbody2D.linearVelocity = Vector2.zero; 
            m_Rigidbody2D.AddForce(force, ForceMode2D.Impulse);
            StartCoroutine(RecoverFromStun());
        }

        private IEnumerator RecoverFromStun()
        {
            yield return new WaitForSeconds(0.3f);
            m_IsStunned = false; 
        }

        public void Move(float move, bool crouch, bool jump)
        {
            if (m_IsStunned)
            {
                return;
            }

            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            m_Anim.SetBool("Crouch", crouch);

            if (m_Grounded || m_AirControl)
            {
                move = (crouch ? move*m_CrouchSpeed : move);

                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                m_Rigidbody2D.linearVelocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.linearVelocity.y);

                if (move > 0 && !m_FacingRight)
                {
                    Flip();
                }
                else if (move < 0 && m_FacingRight)
                {
                    Flip();
                }

                // --- ЛОГИКА ЗВУКА ШАГОВ ---
                // Если мы на земле И мы движемся (скорость > 0)
                if (m_Grounded && Mathf.Abs(move) > 0.1f)
                {
                    m_StepCycle -= Time.deltaTime; // Тикаем таймер назад

                    if (m_StepCycle <= 0)
                    {
                        PlayFootStepAudio();
                        m_StepCycle = m_StepInterval; // Сброс таймера
                    }
                }
                else
                {
                    // Если остановились или в воздухе — сбрасываем таймер на 0, 
                    // чтобы при следующем шаге звук был мгновенным
                    m_StepCycle = 0f;
                }
                // --------------------------
            }

            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
        }

        private void PlayFootStepAudio()
        {
            if (m_FootstepSounds == null || m_FootstepSounds.Length == 0) return;

            // Выбираем случайный звук из списка (чтобы не звучало как робот)
            int n = UnityEngine.Random.Range(0, m_FootstepSounds.Length);
            
            // Немного меняем питч (высоту), чтобы каждый шаг звучал уникально
            m_AudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            
            m_AudioSource.PlayOneShot(m_FootstepSounds[n]);
        }

        private void Flip()
        {
            m_FacingRight = !m_FacingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}