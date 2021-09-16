using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float jump = 5;

    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_IsJumping = false;
    
    private static readonly int Running = Animator.StringToHash("Running");

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleRunning();
        HandleJumping();
    }

    private void HandleJumping()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || m_IsJumping) return;
        
        m_Rigidbody2D.AddForce(new Vector2(0, jump), ForceMode2D.Impulse);
        m_IsJumping = true;
        UpdateJumpingAnim();
    }

    private void UpdateJumpingAnim()
    {
        m_Animator.SetBool("Jumping", m_IsJumping);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            m_IsJumping = false;
            UpdateJumpingAnim();
        }
    }

    #region Running
    private void HandleRunning()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var xAxis = speed * horizontalAxis * Time.deltaTime;
        transform.Translate(xAxis, 0, 0);
        FlipSprite(horizontalAxis);
    }

    private void FlipSprite(float axis)
    {
        int direction;
        if (axis > 0)
        {
            direction = 1;
        }
        else if (axis < 0)
        {
            direction = -1;
        }
        else
        {
            TriggerRunningAnim(false);
            return;
        }

        TriggerRunningAnim(true);
        transform.localScale = new Vector3(direction, 1, 1);
    }

    private void TriggerRunningAnim(bool isRunning)
    {
        m_Animator.SetBool(Running, isRunning);
    }
    #endregion
   
    
}