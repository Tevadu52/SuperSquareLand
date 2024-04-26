﻿using UnityEngine;

public class CameraProfile : MonoBehaviour
{
    [Header("Type")]
    [SerializeField]private CameraProfileType _cameraProfilType = CameraProfileType.Static;

    [Header("Follow")]
    [SerializeField] private Transform _targetToFollow = null;

    private Camera _camera;

    public float CameraSize => _camera.orthographicSize;

    public Vector3 Position => _camera.transform.position;

    public CameraProfileType ProfileType => _cameraProfilType;

    public Transform TargetToFollow => _targetToFollow;


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