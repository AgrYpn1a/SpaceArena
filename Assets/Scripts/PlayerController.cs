using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float boostSpeed;
    [SerializeField]
    private float maxAcceleration;
    [SerializeField]
    private float rotationSpeed = 1.0f;

    // shoot
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private Transform gunRight;
    [SerializeField]
    private Transform gunLeft;

    // controls
    [SerializeField]
    private KeyCode inputShoot;

    private Camera _camera;

    void Awake()
    {
    }

    void Start()
    {

    }

    // movement
    private Vector2 mousePos;
    private Vector2 position;
    private Vector2 targetPos;
    private Vector2 direction;

    private bool isMoving;

    private float timeStarted;
    private float timePassed;

    // rotation
    private float targetAngle;
    private Quaternion targetRotation;

    void Update()
    {
        // pre calculate needed things for momvement & rotation
        mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        position.x = transform.position.x;
        position.y = transform.position.y;

        direction = (mousePos - position).normalized;

        // movement
        if (Input.GetMouseButton(1))
            transform.Translate(transform.up * speed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(inputShoot))
            Shoot();

        // calculate rotation & rotate
        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(targetAngle, transform.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    }

    private void Shoot()
    {
        GameObject ls = Instantiate(laser, gunLeft.position, transform.rotation);
        ls.transform.parent = transform;
        ls = Instantiate(laser, gunRight.position, transform.rotation);
        ls.transform.parent = transform;
    }

    public void SetCamera(GameObject camera)
    {
        this.camera = camera;

        // tell camera about self... {to do}
        // temp
        _camera = camera.GetComponent<Camera>();
    }
}
