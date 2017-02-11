using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float boostSpeed;
    [SerializeField]
    private float maxAcceleration;
    [SerializeField]
    private float rotationSpeed = 1.0f;

    void Awake()
    {
        normalSpeed = speed;
    }

    void Start()
    {

    }

    // movement
    private Vector2 mousePos;
    private Vector2 position;
    private Vector2 targetPos;
    private Vector2 distance;
    private float acc;
    private float normalSpeed;

    private bool isMoving;

    // rotation
    private float targetAngle;
    private Quaternion targetRotation;

    void Update()
    {
        mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        position.x = transform.position.x;
        position.y = transform.position.y;

        Vector2 direction = (mousePos - position).normalized;

        // acceleration based on mouse distance
        acc = Mathf.Abs(mousePos.magnitude - position.magnitude);
        acc = Mathf.Clamp(acc, 1, maxAcceleration); // min acceleration is 1, affects nothing

        //speed = normalSpeed;

        if (Input.GetMouseButton(1))
        {
            speed += boostSpeed * Time.deltaTime;
            speed = Mathf.Clamp(speed, normalSpeed, boostSpeed);

        }

        speed -= Time.deltaTime;
        speed = Mathf.Clamp(speed, normalSpeed, boostSpeed);

        distance = mousePos - position;

        if (distance.magnitude < 2)
            speed = 0;
        
        // calculate rotation & rotate
        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(targetAngle, transform.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // finally, move 
        //transform.Translate(transform.up * speed * Time.deltaTime, Space.World);

        transform.position = Vector2.Lerp(transform.position, mousePos, speed * Time.deltaTime);
    }

    private void InitMovement()
    {
        targetPos = mousePos;
        isMoving = true;
    }

    private void Move()
    {

    }
}
