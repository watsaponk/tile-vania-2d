using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float maxSpeed = 15;
    [SerializeField] private LayerMask groundLayer;

    [Header("Vertical Movement")] public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float _jumpTimer;

    [Header("Physics")] public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;
    public float groundLength = 0.6f;
    public Vector3 collisionOffset;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private float _xDirection;
    [SerializeField] private bool _isOnGround;

    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Falling = Animator.StringToHash("Falling");
    private static readonly int Jumping = Animator.StringToHash("Jumping");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var isLeftFootOnGround = CheckFootOnGround(-1);
        var isRightFootOnGround = CheckFootOnGround(1);
        _isOnGround = isLeftFootOnGround || isRightFootOnGround;
        _xDirection = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isOnGround)
        {
            _jumpTimer = Time.time + jumpDelay;
        }
    }

    private bool CheckFootOnGround(float side)
    {
        return Physics2D.Raycast(
            origin: (transform.position + collisionOffset) * side,
            direction: Vector2.down,
            distance: groundLength,
            layerMask: groundLayer
        );
    }

    private void FixedUpdate()
    {
        ModifyPhysics();
        HandleRunning();

        if (_jumpTimer > Time.time)
        {
            HandleJumping();
        }
    }

    void ModifyPhysics()
    {
        var velocity = _rigidbody.velocity;
        var changingDirections = (_xDirection > 0 && velocity.x < 0) || (_xDirection < 0 && velocity.x > 0);

        if (_isOnGround)
        {
            if (Mathf.Abs(_xDirection) < 0.4f || changingDirections)
            {
                _rigidbody.drag = linearDrag;
            }
            else
            {
                _rigidbody.drag = 0f;
            }

            _rigidbody.gravityScale = 0;
        }
        else
        {
            _rigidbody.gravityScale = gravity;
            _rigidbody.drag = linearDrag * 0.15f;
            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.gravityScale = gravity * fallMultiplier;
            }
            else if (_rigidbody.velocity.y > 0 && !Input.GetKeyDown(KeyCode.Space))
            {
                _rigidbody.gravityScale = gravity * (fallMultiplier / 2);
            }
        }

        HandleVerticalMovementAnimation();
    }


    #region Jumping

    private void HandleVerticalMovementAnimation()
    {
        if (_isOnGround)
        {
            UpdateJumpingAnim(false);
            UpdateFallingAnim(false);
            return;
        }

        var yVelocity = _rigidbody.velocity.y;
        if (yVelocity > 0)
        {
            UpdateJumpingAnim(true);
            UpdateFallingAnim(false);
        }
        else
        {
            UpdateJumpingAnim(false);
            UpdateFallingAnim(true);
        }
    }

    private void UpdateFallingAnim(bool isFalling)
    {
        _animator.SetBool(Falling, isFalling);
    }

    private void HandleJumping()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        _jumpTimer = 0;
    }

    private void UpdateJumpingAnim(bool isJumping)
    {
        _animator.SetBool(Jumping, isJumping);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawLine(position + collisionOffset, position + collisionOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(position - collisionOffset, position - collisionOffset + Vector3.down * groundLength);
    }

    #endregion

    #region Running

    private void HandleRunning()
    {
        _rigidbody.AddForce(Vector2.right * _xDirection);
        _rigidbody.velocity = new Vector2(_xDirection * speed, _rigidbody.velocity.y);

        if (_xDirection == 0 && _isOnGround)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }

        if (Mathf.Abs(_rigidbody.velocity.x) > maxSpeed)
        {
            _rigidbody.velocity = new Vector2(Mathf.Sign(_rigidbody.velocity.x) * maxSpeed, _rigidbody.velocity.y);
        }

        FlipSprite(_xDirection);
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
        _animator.SetBool(Running, isRunning);
    }

    #endregion
}