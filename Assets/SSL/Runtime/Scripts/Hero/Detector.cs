using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [System.Serializable]
    public struct Detection
    {
        public Vector2 direction;
        public Transform[] detectionPoints;
        public float detectionLength;
        public LayerMask layerMask;
    }

    [Header("Detections")]
    [SerializeField] private Detection[] _detections;

    private Detection GetDetection(Vector2 direction)
    {
        Detection detection = _detections[0];
        foreach (Detection detect in _detections)
        {
            if (detect.direction == direction)
            {
                detection = detect;
            }
        }
        return detection;
    }

    public bool DetectNearBy(Vector2 direction)
    {
        if (_detections.Length == 0)
        {
            Debug.LogWarning($"Attention, le gameObject {gameObject.name} possède un detector mais ne s'en sert pas");
            return false;
        }
        Detection detection = GetDetection(direction);
        
        foreach (Transform detectionPoint in detection.detectionPoints)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                    detectionPoint.position,
                    direction,
                    detection.detectionLength,
                    detection.layerMask
                );

            if (hitResult.collider != null)
            {
                return true;
            }
        }

        return false;
    }
}
