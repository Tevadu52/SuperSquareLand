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

        if (GetInputDash())
        {
            if(!_entity.IsDashing)
            {
                _entity.StartDash();
            }
        }
        if (GetInputJump())
        {
            if (_entity.IsTouchingGround && !_entity.IsJumping)
            {
                _entity.StartJump();
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

    private bool GetInputDash()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private bool GetInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.EndVertical();
    }
}