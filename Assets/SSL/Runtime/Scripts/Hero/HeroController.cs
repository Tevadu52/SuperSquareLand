using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void Update()
    {
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

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.EndVertical();
    }
}