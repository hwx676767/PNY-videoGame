using UnityEngine;

/// <summary>
/// 俯视角相机跟随脚本 - 相机固定在物体上方
/// </summary>
public class TopDownCameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField]
    private Transform _target;              // 要跟随的目标对象
    
    [Header("相机位置")]
    [SerializeField]
    private Vector3 _offset = new Vector3(0, 10, -5);  // 相对于目标的偏移量
    
    [SerializeField]
    private bool _fixedAngle = true;         // 是否固定相机角度
    
    [SerializeField]
    private Vector3 _rotationAngle = new Vector3(45, 0, 0);  // 相机角度（俯视角常用45度）
    
    [Header("跟随平滑度")]
    [SerializeField]
    private bool _smoothFollow = true;        // 是否平滑跟随
    
    [SerializeField]
    [Range(0.1f, 10f)]
    private float _smoothSpeed = 5f;          // 跟随平滑速度
    
    [Header("边界限制")]
    [SerializeField]
    private bool _useBoundaries = false;      // 是否使用边界限制
    
    [SerializeField]
    private Vector2 _xBounds = new Vector2(-20, 20);  // X轴移动范围
    
    [SerializeField]
    private Vector2 _zBounds = new Vector2(-20, 20);  // Z轴移动范围
    
    private Vector3 _velocity = Vector3.zero;
    
    void Start()
    {
        // 如果没有指定目标，尝试找玩家
        if (_target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _target = player.transform;
        }
        
        // 初始化相机位置
        if (_target != null)
        {
            transform.position = GetTargetPosition();
            
            if (_fixedAngle)
            {
                transform.rotation = Quaternion.Euler(_rotationAngle);
            }
        }
    }
    
    void LateUpdate()
    {
        if (_target == null) return;
        
        // 计算目标位置
        Vector3 targetPosition = GetTargetPosition();
        
        // 应用边界限制
        if (_useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, _xBounds.x, _xBounds.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, _zBounds.x, _zBounds.y);
        }
        
        // 平滑跟随或直接跟随
        if (_smoothFollow)
        {
            // 使用SmoothDamp实现平滑跟随
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref _velocity, 
                1f / _smoothSpeed
            );
        }
        else
        {
            transform.position = targetPosition;
        }
        
        // 固定相机角度
        if (_fixedAngle)
        {
            transform.rotation = Quaternion.Euler(_rotationAngle);
        }
    }
    
    /// <summary>
    /// 获取目标位置（目标位置 + 偏移量）
    /// </summary>
    private Vector3 GetTargetPosition()
    {
        return _target.position + _offset;
    }
    
    /// <summary>
    /// 设置新的跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }
    
    /// <summary>
    /// 设置相机偏移量
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        _offset = newOffset;
    }
    
    // 在Scene视图中显示相机范围和目标
    private void OnDrawGizmosSelected()
    {
        if (_target == null) return;
        
        // 显示相机目标点
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_target.position, 0.5f);
        
        // 显示相机位置
        Vector3 cameraPos = GetTargetPosition();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(cameraPos, 0.3f);
        Gizmos.DrawLine(_target.position, cameraPos);
        
        // 显示边界范围
        if (_useBoundaries)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3(
                (_xBounds.x + _xBounds.y) / 2f,
                0,
                (_zBounds.x + _zBounds.y) / 2f
            );
            Vector3 size = new Vector3(
                _xBounds.y - _xBounds.x,
                1,
                _zBounds.y - _zBounds.x
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}