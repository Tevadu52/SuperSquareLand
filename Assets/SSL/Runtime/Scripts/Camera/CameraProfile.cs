using UnityEngine;

public class CameraProfile : MonoBehaviour
{
    [Header("Type")]
    [SerializeField]private CameraProfileType _cameraProfilType = CameraProfileType.Static;

    [Header("Follow")]
    [SerializeField] private CameraFollowable _targetToFollow = null;

    [Header("Damping")]
    [SerializeField] private bool _useDampingHorizontally = false;
    [SerializeField] private float _horizontalDampingFactor = 5f;
    [SerializeField] private bool _useDampingVertically = false;
    [SerializeField] private float _verticalDampingFactor = 5f;

    private Camera _camera;
    public float CameraSize => _camera.orthographicSize;
    public Vector3 Position => _camera.transform.position;
    public CameraProfileType ProfileType => _cameraProfilType;
    public CameraFollowable TargetToFollow => _targetToFollow;
    public bool UseDampingHorizontally => _useDampingHorizontally;
    public float HorizontalDampingFactor => _horizontalDampingFactor;
    public bool UseDampingVertically => _useDampingVertically;
    public float VerticalDampingFactor => _verticalDampingFactor;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if ( _camera != null )  _camera.enabled = false;
    }
}


public enum CameraProfileType
{
    Static = 0,
    FollowTarget,
}