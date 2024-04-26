using UnityEngine;

public class CameraProfile : MonoBehaviour
{
    [Header("Type")]
    [SerializeField]private CameraProfileType _cameraProfilType = CameraProfileType.Static;

    [Header("Follow")]
    [SerializeField] private GameObject _objectToFollow = null;
    [SerializeField] private float _followOffsetX = 8f;
    [SerializeField] private float _followOffsetDamping = 1.5f;

    [Header("Damping")]
    [SerializeField] private bool _useDampingHorizontally = false;
    [SerializeField] private float _horizontalDampingFactor = 5f;
    [SerializeField] private bool _useDampingVertically = false;
    [SerializeField] private float _verticalDampingFactor = 5f;

    [Header("Bounds")]
    [SerializeField] private bool _asBounds = false;
    [SerializeField] private Rect _boundsRect = new Rect(0f,0f,10f,10f);


    private Camera _camera;
    public float CameraSize => _camera.orthographicSize;
    public Vector3 Position => _camera.transform.position;
    public CameraProfileType ProfileType => _cameraProfilType;
    public float FollowOffsetX => _followOffsetX;
    public float FollowOffsetDamping => _followOffsetDamping;
    public GameObject ObjectToFollow => _objectToFollow;
    public CameraFollowable TargetToFollow => _objectToFollow.GetComponent<CameraFollowable>();
    public bool UseDampingHorizontally => _useDampingHorizontally;
    public float HorizontalDampingFactor => _horizontalDampingFactor;
    public bool UseDampingVertically => _useDampingVertically;
    public float VerticalDampingFactor => _verticalDampingFactor;
    public bool AsBounds => _asBounds;
    public Rect BoundsRect => _boundsRect;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if ( _camera != null )  _camera.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!AsBounds) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_boundsRect.center, _boundsRect.size);
    }
}


public enum CameraProfileType
{
    Static = 0,
    FollowTarget,
}