using UnityEngine;
using Cinemachine;

/// <summary>
/// 双模式相机控制脚本 - 支持俯视角跟随和第一人称视角切换
/// </summary>
public class DualModeCameraController : MonoBehaviour
{
    [Header("相机组件")]
    [SerializeField]
    private CinemachineVirtualCamera _topDownVCam;      // 俯视角虚拟相机
    
    [SerializeField]
    private CinemachineVirtualCamera _firstPersonVCam;  // 第一人称虚拟相机
    
    [Header("跟随目标")]
    [SerializeField]
    private Transform _target;              // 要跟随的目标对象（玩家）
    
    [Header("俯视角设置")]
    [SerializeField]
    private Vector3 _topDownOffset = new Vector3(0, 10, -5);  // 相对于目标的偏移量
    
    [SerializeField]
    private Vector3 _topDownRotation = new Vector3(45, 0, 0);  // 相机角度（俯视角常用45度）
    
    
    [Header("切换设置")]
    [SerializeField]
    private KeyCode _switchKey = KeyCode.Tab; // 切换相机键
    
    [Header("边界限制（俯视角模式）")]
    [SerializeField]
    private bool _useBoundaries = false;      // 是否使用边界限制
    
    [SerializeField]
    private Vector2 _xBounds = new Vector2(-20, 20);  // X轴移动范围
    
    [SerializeField]
    private Vector2 _zBounds = new Vector2(-20, 20);  // Z轴移动范围
    
    // 私有变量
    private bool _isFirstPerson = false;      // 当前是否为第一人称模式
    private CinemachineBrain _brain;           // 相机大脑
    private float _currentX = 0f;               // 当前水平旋转角度
    private float _currentY = 0f;               // 当前垂直旋转角度
    
    // 用于平滑俯视角跟随的变量
    private Vector3 _smoothedTargetPosition;
    private Vector3 _velocity = Vector3.zero;
    
    [Header("俯视角平滑设置")]
    [SerializeField]
    private bool _smoothTopDownFollow = true;   // 是否平滑俯视角跟随
    [SerializeField]
    private float _topDownSmoothSpeed = 5f;     // 俯视角平滑速度
    
    void Start()
    {
        InitializeCamera();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void LateUpdate()
    {
        if (!_isFirstPerson)
        {
            HandleTopDownFollow();
        }
    }
    
    /// <summary>
    /// 初始化相机设置
    /// </summary>
    private void InitializeCamera()
    {
        // 如果没有指定目标，尝试找玩家
        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _target = player.transform;
        }
        
        if (_target == null)
        {
            Debug.LogError("未找到跟随目标！请指定Target或确保玩家有'Player'标签");
            return;
        }
        
        // 获取或创建CinemachineBrain
        _brain = Camera.main.GetComponent<CinemachineBrain>();
        if (_brain == null)
        {
            _brain = Camera.main.gameObject.AddComponent<CinemachineBrain>();
            _brain.m_DefaultBlend.m_Time = 0.5f; // 设置默认切换时间
        }
        
        // 初始化平滑位置
        _smoothedTargetPosition = _target.position;
        
        
        // 创建或设置虚拟相机
        SetupVirtualCameras();
        
        // 默认启用俯视角
        SetCameraMode(false);
        
        // 初始锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    
    /// <summary>
    /// 设置虚拟相机
    /// </summary>
    private void SetupVirtualCameras()
    {
        // 如果没有俯视角相机，创建一个
        if (_topDownVCam == null)
        {
            _topDownVCam = CreateVirtualCamera("TopDownVCam", 10);
        }
        
        // 如果没有第一人称相机，创建一个
        if (_firstPersonVCam == null)
        {
            _firstPersonVCam = CreateVirtualCamera("FirstPersonVCam", 0);
        }
        
        // 配置俯视角相机
        ConfigureTopDownCamera();
        
        // 配置第一人称相机
        ConfigureFirstPersonCamera();
    }
    
    /// <summary>
    /// 创建虚拟相机
    /// </summary>
    private CinemachineVirtualCamera CreateVirtualCamera(string name, int priority)
    {
        GameObject vCamObj = new GameObject(name);
        vCamObj.transform.parent = transform;
        
        var vcam = vCamObj.AddComponent<CinemachineVirtualCamera>();
        vcam.m_Priority = priority;
        
        return vcam;
    }
    
    /// <summary>
    /// 配置俯视角相机 - 使用平滑跟随来避免波动
    /// </summary>
    private void ConfigureTopDownCamera()
    {
        if (_topDownVCam == null || _target == null) return;
        
        // 俯视角相机跟随一个虚拟点，而不是直接跟随目标
        // 这样可以过滤掉目标的波动
        GameObject followPoint = new GameObject("TopDownFollowPoint");
        followPoint.transform.parent = transform;
        _topDownVCam.Follow = followPoint.transform;
        _topDownVCam.LookAt = _target; // LookAt仍然看着目标，保持朝向
        
        // 获取或添加Framing Transposer组件
        var transposer = _topDownVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer == null)
        {
            transposer = _topDownVCam.AddCinemachineComponent<CinemachineFramingTransposer>();
        }
        
        // 设置相机偏移
        transposer.m_TrackedObjectOffset = _topDownOffset;
        
        // 设置相机旋转
        _topDownVCam.transform.rotation = Quaternion.Euler(_topDownRotation);
        
        // 禁用屏幕X和Y的移动（保持固定偏移）
        transposer.m_XDamping = 0;
        transposer.m_YDamping = 0;
        transposer.m_ZDamping = 0;
        transposer.m_DeadZoneWidth = 0;
        transposer.m_DeadZoneHeight = 0;
        transposer.m_SoftZoneWidth = 0;
        transposer.m_SoftZoneHeight = 0;
    }
    
    /// <summary>
    /// 配置第一人称相机 - 直接跟随挂载点，保留所有波动
    /// </summary>
    private void ConfigureFirstPersonCamera()
    {
        if (_firstPersonVCam == null) return;
        
        _firstPersonVCam.LookAt = null; // 第一人称不需要LookAt
        
        // 获取或添加POV组件用于视角控制
        var pov = _firstPersonVCam.GetCinemachineComponent<CinemachinePOV>();
        if (pov == null)
        {
            pov = _firstPersonVCam.AddCinemachineComponent<CinemachinePOV>();
        }
        
        // 设置POV参数（我们将手动控制，所以禁用自动旋转）
        pov.m_HorizontalAxis.m_MaxSpeed = 0;
        pov.m_VerticalAxis.m_MaxSpeed = 0;
        
        // 初始角度
        _currentX = _target.eulerAngles.y;
        _currentY = 0;
        
        // 设置相机位置（直接在挂载点位置）
        //_firstPersonVCam.transform.localPosition = Vector3.zero;
        _firstPersonVCam.transform.localPosition = new Vector3(0, 1, 0);
        _firstPersonVCam.transform.localRotation = Quaternion.identity;
    }
    
    /// <summary>
    /// 处理输入
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown(_switchKey))
        {
            _isFirstPerson = !_isFirstPerson;
            SetCameraMode(_isFirstPerson);
        }
        
        // 鼠标锁定控制
        if (_isFirstPerson)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    
    /// <summary>
    /// 设置相机模式
    /// </summary>
    private void SetCameraMode(bool firstPerson)
    {
        if (_topDownVCam != null)
        {
            _topDownVCam.Priority = firstPerson ? 0 : 10;
        }
        
        if (_firstPersonVCam != null)
        {
            _firstPersonVCam.Priority = firstPerson ? 10 : 0;
        }
        
        
        // 切换到第一人称时，确保相机位置正确
        if (firstPerson)
        {
            _currentX = _target.eulerAngles.y;
            _currentY = 1;
        }
    }
    
    
    /// <summary>
    /// 处理俯视角跟随 - 平滑跟随，过滤波动
    /// </summary>
    private void HandleTopDownFollow()
    {
        if (_target == null || _topDownVCam == null) return;
        
        // 应用边界限制
        Vector3 targetPos = _target.position;
        if (_useBoundaries)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, _xBounds.x, _xBounds.y);
            targetPos.z = Mathf.Clamp(targetPos.z, _zBounds.x, _zBounds.y);
        }
        
        // 平滑处理位置，过滤掉高频波动
        if (_smoothTopDownFollow)
        {
            // 使用SmoothDamp平滑过渡，只跟随低频移动
            _smoothedTargetPosition = Vector3.SmoothDamp(
                _smoothedTargetPosition,
                targetPos,
                ref _velocity,
                1f / _topDownSmoothSpeed
            );
        }
        else
        {
            _smoothedTargetPosition = targetPos;
        }
        
        // 更新俯视角相机的跟随点位置
        if (_topDownVCam.Follow != null)
        {
            _topDownVCam.Follow.position = _smoothedTargetPosition;
        }
        
        // 更新相机偏移（如果需要动态修改）
        var transposer = _topDownVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset = _topDownOffset;
        }
    }
    
    /// <summary>
    /// 设置新的跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
        
        // 重新创建挂载点和配置相机
        if (_target != null)
        {
            _smoothedTargetPosition = _target.position;
            ConfigureTopDownCamera();
            ConfigureFirstPersonCamera();
        }
    }
    
    /// <summary>
    /// 切换到指定模式
    /// </summary>
    public void SwitchToMode(bool firstPerson)
    {
        if (_isFirstPerson != firstPerson)
        {
            _isFirstPerson = firstPerson;
            SetCameraMode(firstPerson);
        }
    }
    
    /// <summary>
    /// 获取当前模式
    /// </summary>
    public bool IsFirstPersonMode()
    {
        return _isFirstPerson;
    }
    
}