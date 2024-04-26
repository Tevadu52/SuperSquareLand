using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    //Camera Follow
    private CameraFollowable _cameraFollowable;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementsSettings")]
    [SerializeField] private HeroHorizontalMovementsSettings _groundHorizontalMovementsSettings;
    [SerializeField] private HeroHorizontalMovementsSettings _airHorizontalmovementsSettings;
    private float _horizontalSpeed;
    private float _moveDirX = 0f;
    private HeroHorizontalMovementsSettings _GetHeroHorizontalMovementsSettings()
    {
        if (IsJumping) 
        {
            if (JumpSettings == _wallJumpSettings) return _wallJumpHorizontalmovementsSettings;
            else return _jumpHorizontalmovementsSettings;
        }
        return IsTouchingGround ? _groundHorizontalMovementsSettings : _airHorizontalmovementsSettings;
    }

    [Header("Dash")]
    [FormerlySerializedAs("_dashSettings")]
    [SerializeField] private HeroDashSettings _groundDashSettings;
    [SerializeField] private HeroDashSettings _airDashSettings;
    private float _dashTimer = 0f;
    private HeroDashSettings _GetHeroDashSettings()
    {
        return IsTouchingGround ? _groundDashSettings : _airDashSettings;
    }
    public bool IsDashing { get; private set; } = false;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Fall")]
    [SerializeField] private HeroFallSetting _fallSettings;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Detector")]
    [SerializeField] private Detector _detector;
    public bool IsTouchingGround { get; private set; } = false;
    public bool IsTouchingRightWall { get; private set; } = false;
    public bool IsTouchingLeftWall { get; private set; } = false;

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings _groundJumpSettings;
    [SerializeField] private HeroFallSetting _jumpFallSettings;
    [SerializeField] private HeroHorizontalMovementsSettings _jumpHorizontalmovementsSettings;

    [Header("AirJump")]
    [FormerlySerializedAs("_allJumpSettings")]
    [SerializeField] private HeroJumpSettings[] _allAirJumpSettings;
    private int _airJumpIndex;

    [Header("WallJump")]
    [SerializeField] private float _wallJumpSpeed;
    [SerializeField] private HeroJumpSettings _wallJumpSettings;
    [SerializeField] private HeroHorizontalMovementsSettings _wallJumpHorizontalmovementsSettings;

    private HeroJumpSettings JumpSettings { get; set; }

    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling,
    }

    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;
    public bool IsJumping => _jumpState != JumpState.NotJumping;
    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    public bool IsJumpFalling => _jumpState == JumpState.Falling;
    public bool IsJumpMinDurationReached => _jumpTimer >= JumpSettings.jumpMinDuration;
    public bool IsLastJumpReached => _airJumpIndex == _allAirJumpSettings.Length - 1;


    [Header("WallSlide")]
    [SerializeField] private HeroFallSetting _wallSlideFallSettings;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void Awake()
    {
        _cameraFollowable = GetComponent<CameraFollowable>();
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        _cameraFollowable.FollowPositionY = _rigidbody.position.y;
    }

    private void FixedUpdate()
    {
        _ApplyGroundDetector();
        _ApplyWallDetector();
        _UpdateCameraFollowPosition();

        HeroHorizontalMovementsSettings horizontalMovementsSettings = _GetHeroHorizontalMovementsSettings();
        HeroDashSettings dashSettings = _GetHeroDashSettings();
        if (_AreOrientAndMovementOpposite())
        {
            turnBack(horizontalMovementsSettings);
        }  else {
            _UpdateHorizontalSpeed(horizontalMovementsSettings);
            _ChangeOrientFromHorizontalMovement();
        }
        if(IsJumping)
        {
            _UpdateJump();
        } else {
            if(!IsTouchingGround && !IsDashing && !_LookAtWall())
            {
                _ApplyFallGravity(_fallSettings);
            } else if (IsTouchingGround)
            {
                _ResetVerticalSpeed();
                _ResetJumpIndex();
            }
        }

        if (!IsTouchingGround && _LookAtWall())
        {
            if (IsJumpFalling) _ResetJumpIndex();
            _StopDash(dashSettings);
            _ApplyFallGravity(_wallSlideFallSettings);
        }

        if (IsDashing)
        {
            _UpdateDash(dashSettings, horizontalMovementsSettings);
        }

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
    }

    #region Dash Move
    public void StartDash()
    {
        IsDashing = true;
        _dashTimer = 0f;
        StopJumpImpulsion();
    }

    private void _UpdateDash(HeroDashSettings dashSettings, HeroHorizontalMovementsSettings movementsSettings)
    {
        _dashTimer += Time.fixedDeltaTime;
        if (_dashTimer < dashSettings.duration)
        {
            _horizontalSpeed = dashSettings.speed;
        } else {
            IsDashing = false;
            _horizontalSpeed = movementsSettings.speedMax;
        }
    }

    public void _StopDash(HeroDashSettings settings)
    {
        _dashTimer = settings.duration;
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

    private void _UpdateHorizontalSpeed(HeroHorizontalMovementsSettings settings)
    {
        if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        } else {
            _Deccelerate(settings);
        }
    }

    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = 0f;
    }
    #endregion Horizontal Move

    #region Speed Change
    private void _Accelerate(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax)
        {
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _Deccelerate(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed -= settings.decceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    private void turnBack(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFriction * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }
    #endregion Speed Change

    #region Fall and Vertical Move

    private void _ApplyFallGravity(HeroFallSetting settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if(_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
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

    #region Jump

    public void StartJump()
    {
        JumpSettings = _groundJumpSettings;
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    public void StartWallJump()
    {
        if(!_LookAtWall()) return;
        JumpSettings = _wallJumpSettings;
        _orientX *= -1;
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    public void StartAirJump()
    {
        if (IsLastJumpReached) return;
        _GetNextJump();
        JumpSettings = _allAirJumpSettings[_airJumpIndex];
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    public void StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }

    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < JumpSettings.jumpMaxDuration)
        {
            _verticalSpeed = JumpSettings.jumpSpeed;
            if (JumpSettings == _wallJumpSettings) _horizontalSpeed = _wallJumpSpeed;
        }
        else
        {
            _jumpState = JumpState.Falling;
        }
    }
    private void _UpdateJumpStateFalling()
    {
        if (!IsTouchingGround && !(IsTouchingRightWall || IsTouchingLeftWall))
        {
            _ApplyFallGravity(_jumpFallSettings);
        } else {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion();
                break; 
            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }

    private void _GetNextJump()
    {
        if(!IsLastJumpReached)
        {
            _airJumpIndex++;
        } 
    }

    private void _ResetJumpIndex()
    {
        _airJumpIndex = -1;
    }
    #endregion Jump

    #region Detector
    private void _ApplyGroundDetector()
    {
        IsTouchingGround = _detector.DetectNearBy(Vector2.down);
    }

    private void _ApplyWallDetector()
    {
        IsTouchingLeftWall = (_detector.DetectNearBy(Vector2.left));
        IsTouchingRightWall = (_detector.DetectNearBy(Vector2.right));
    }
    #endregion Detector

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

    private bool _LookAtWall()
    {
        if (IsTouchingLeftWall && _orientX == -1)
        {
            return true;
        } 
        else if (IsTouchingRightWall && _orientX == 1)
        {
            return true;
        } else {
            return false;
        }
    }
    #endregion Orientation

    private void _UpdateCameraFollowPosition()
    {
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        if(IsTouchingGround || IsTouchingLeftWall || IsTouchingRightWall)
        {
            _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        }
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        if (IsTouchingGround)
        {
            GUILayout.Label($"OnGround");
        } else {
            GUILayout.Label($"InAir");
        }

        if (IsTouchingLeftWall)
        {
            GUILayout.Label($"TouchingLeftWall");
        } else {
            GUILayout.Label($"notTouchingLeftWall");
        }
        if (IsTouchingRightWall)
        {
            GUILayout.Label($"TouchingRightWall");
        } else {
            GUILayout.Label($"notTouchingRightWall");
        }
        GUILayout.Label($"JumpState = {_jumpState}");
        GUILayout.Label($"AirJumpIndex = {_airJumpIndex + 1}");
        GUILayout.EndVertical();
    }
}