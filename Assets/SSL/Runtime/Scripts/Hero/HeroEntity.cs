using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [SerializeField] private HeroHorizontalMovementsSettings _movementsSettings;
    private float _horizontalSpeed;
    private float _moveDirX = 0f;

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSettings;
    private float timeDash = 0f;
    private bool isDashing = false;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSetting _fallSettings;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; private set; } = false;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;


    private void FixedUpdate()
    {
        _ApplyGroundDetector();

        if(_AreOrientAndMovementOpposite())
        {
            turnBack();
        }  else {
            _UpdateHorizontalSpeed();
            _ChangeOrientFromHorizontalMovement();
        }
        if(!IsTouchingGround)
        {
            _ApplyFallGravity();
        } else {
            _ResetVerticalSpeed();
        }
        
        _ApplyHorizontalSpeed();
        _UpdateDash();
        _ApplyVerticalSpeed();
    }

    #region Dash Move
    public void StartDash()
    {
        timeDash = 0f;
        isDashing = true;
    }

    private void _UpdateDash()
    {
        if (isDashing)
        {
            timeDash += Time.fixedDeltaTime;
            _horizontalSpeed = _dashSettings.speed;
        }
        if (isDashing && timeDash > _dashSettings.duration)
        {
            isDashing = false;
            _horizontalSpeed = _movementsSettings.speedMax;
        }
    }
    #endregion Dash Move

    #region Horizontal Move
    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private void _UpdateHorizontalSpeed()
    {
        if (isDashing) return;
        if (_moveDirX != 0f)
        {
            _Accelerate();
        } else {
            _Deccelerate();
        }
    }
    #endregion Horizontal Move

    #region Speed Change
    private void _Accelerate()
    {
        _horizontalSpeed += _movementsSettings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > _movementsSettings.speedMax)
        {
            _horizontalSpeed = _movementsSettings.speedMax;
        }
    }

    private void _Deccelerate()
    {
        _horizontalSpeed -= _movementsSettings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    private void turnBack()
    {
        _horizontalSpeed -= _movementsSettings.turnBackFriction * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }
    #endregion Speed Change

    #region Fall and Vertical Move

    private void _ApplyFallGravity()
    {
        _verticalSpeed -= _fallSettings.fallGravity * Time.fixedDeltaTime;
        if(_verticalSpeed < -_fallSettings.fallSpeedMax)
        {
            _verticalSpeed = -_fallSettings.fallSpeedMax;
        }
    }
    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }

    #endregion Fall and Vertical Move

    private void _ApplyGroundDetector()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }

    private void Update()
    {
        _UpdateOrientVisual();
    }

    #region Orientation
    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }
    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }
    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }
    #endregion Orientation

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if(IsTouchingGround)
        {
            GUILayout.Label($"OnGround");
        } else {
            GUILayout.Label($"InAir");
        }
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}