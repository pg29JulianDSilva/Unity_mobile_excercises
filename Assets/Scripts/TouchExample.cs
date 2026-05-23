//Imports necesary for touch
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchExample : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _moveSpeed = 15f;

    private Vector3 targetPosition;
    private bool isDragging;
    
    //Activate and initialize the Touch
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    
    //And Deactivate
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouch();
        MoveToTarget();
    }
    
    private void HandleTouch()
    {
        //The touch register is basically a list with all the number of interactions
        if (Touch.activeTouches.Count == 0)
        {
            isDragging = false;
            return;
        }
        
        var touch = Touch.activeTouches[0];
        if (touch.phase == TouchPhase.Began)
        {
            targetPosition = ScreenToWorld(touch.screenPosition);
            isDragging = true;
        } 
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            targetPosition = ScreenToWorld(touch.screenPosition);
        } 
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * _moveSpeed);
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        float z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, z));
    }
}
