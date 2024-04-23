using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Jump Buffer")]
    [SerializeField] private float _jumpBufferDuration = 0.2f;
    private float _jumpBufferTimer = 0f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void Start()
    {
        _CancelJumpBuffer();
    }

    private void Update()
    {
        _UpdateJumpBuffer();

        _entity.SetMoveDirX(GetInputMoveX()); 

        if (GetInputDownDash())
        {
            if(!_entity.IsDashing)
            {
                _entity.StartDash();
            }
        }

        if (GetInputDownJump())
        {
            if (_entity.IsTouchingGround && !_entity.IsJumping)
            {
                _entity.StartJump();
            } else {
                _ResetJumpBuffer();
            }
        }

        if(_IsJumpBufferActive())
        {
            if (_entity.IsTouchingGround && !_entity.IsJumping)
            {
                _entity.StartJump();
            }
        }

        if (_entity.IsJumpImpulsing)
        {
            if (!GetInputJump() && _entity.IsJumpMinDurationReached)
            {
                _entity.StopJumpImpulsion();
            }
        }
    }

    #region Input
    private float GetInputMoveX()
    {
        float inputMoveX = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            inputMoveX = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMoveX = 1f;
        }

        return inputMoveX;
    }

    private bool GetInputDownDash()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private bool GetInputDownJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool GetInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }
    #endregion Input

    #region Jump Buffer

    private void _ResetJumpBuffer()
    {
        _jumpBufferTimer = 0f;
    }
    private void _UpdateJumpBuffer()
    {
        if(!_IsJumpBufferActive()) return;
        _jumpBufferTimer += Time.deltaTime;
    }
    private bool _IsJumpBufferActive()
    {
        return _jumpBufferTimer < _jumpBufferDuration;
    }
    private void _CancelJumpBuffer()
    {
        _jumpBufferTimer = _jumpBufferDuration;
    }

    #endregion Jump Buffer

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump Buffer Timer = {_jumpBufferTimer}");
        GUILayout.EndVertical();
    }
}