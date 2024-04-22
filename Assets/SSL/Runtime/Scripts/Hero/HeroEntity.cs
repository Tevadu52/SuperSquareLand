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
    private bool doDash = false;
    private float timeDash = 0f;
    private bool isDashing = false;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    public void SetDash()
    {
        doDash = true;
    }

    private void FixedUpdate()
    {
        if(_AreOrientAndMovementOpposite())
        {
            turnBack();
        }  else {
            _UpdateHorizontalSpeed();
            _ChangeOrientFromHorizontalMovement();
        }
        _ApplyHorizontalSpeed();
        updateDash();
    }
    private void updateDash()
    {
        if (doDash)
        {
            doDash = false;
            timeDash = 0f;
            isDashing = true;
        }
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

    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
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

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.EndVertical();
    }
}