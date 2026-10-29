using UnityEngine;

/// <summary>
/// 第三人称角色移动（摄像机方向 + 自动转向）
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MoveController : MonoBehaviour
{
    [Header("移动")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;

    [Header("旋转")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("设置")]
    [SerializeField] private bool useCameraDirection = true;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;

    private Vector3 currentVelocity;
    private bool inputEnabled = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inputEnabled = !inputEnabled;
            Debug.Log("移动输入：" + (inputEnabled ? "开启" : "关闭"));
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (!inputEnabled)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.fixedDeltaTime * acceleration);
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = GetMoveDirection(horizontal, vertical);

        Vector3 targetVelocity = direction * moveSpeed;

        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            Time.fixedDeltaTime * acceleration
        );

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);

        RotateCharacter(direction);
    }

    Vector3 GetMoveDirection(float horizontal, float vertical)
    {
        Vector3 direction;

        if (useCameraDirection && cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            direction = forward * vertical + right * horizontal;
        }
        else
        {
            direction = new Vector3(horizontal, 0, vertical);
        }

        return direction.normalized;
    }

    void RotateCharacter(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    public bool IsMoving()
    {
        return currentVelocity.magnitude > 0.1f;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public bool IsInputEnabled()
    {
        return inputEnabled;
    }
}