using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class GyroStuff : MonoBehaviour
{
    [Header("Tilt elements")]
    [SerializeField] private float tiltForce = 20f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float smoothing = 0.15f;
    
    [Header("Rotation elements")]
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    
    [Header("In Game Info")]
    [SerializeField] private float scoreRadius = 1f;
    [SerializeField] private int rounds = 5;
    
    public static int Score { get; private set; }
    
    private Rigidbody _rb;
    private Vector3 _spawnPosition;
    private Vector3 _smoothedAccel;
    private GameObject _currentRound;
    private int _circlesCollected;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _spawnPosition = transform.position;
        Score = 0;
        
        if( Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
    }

    private void FixedUpdate()
    {
        ApplyTilt();
    }

    private void ApplyTilt()
    {
        if (Accelerometer.current == null) return;
        
        Vector3 raw = Accelerometer.current.acceleration.ReadValue();
        _smoothedAccel = Vector3.Lerp(_smoothedAccel, raw, smoothing);
        
        float x = invertX ? _smoothedAccel.x : -_smoothedAccel.x;
        float z = invertY ? _smoothedAccel.y : -_smoothedAccel.y;
        
        _rb.AddForce(new Vector3(x, 0, z) * tiltForce, ForceMode.Acceleration);
    }
}