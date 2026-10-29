using UnityEngine;
using System.Collections;

public class WhaleController : MonoBehaviour
{
    [Header("跳跃参数设置")]
    [Tooltip("整个跳跃持续时间（秒）")]
    public float jumpDuration = 4f;

    [Tooltip("水平前进距离（米）")]
    public float forwardDistance = 15f;

    [Tooltip("跳跃高度（相对于起始深度的最大上升高度）")]
    public float jumpHeight = 8f;

    [Tooltip("跳跃触发按键（可选，用于测试）")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("调试")]
    [Tooltip("是否正在跳跃")]
    public bool isJumping = false;

    private Vector3 startPosition;
    private Vector3 horizontalDir;
    private float startY;
    private Coroutine jumpCoroutine;

    /// <summary>
    /// 手动触发鲸鱼跳跃（可在其他脚本中调用）
    /// </summary>
    public void StartJump()
    {
        if (isJumping)
        {
            Debug.Log("直接返回");
            return;
        }
        
        Debug.Log("跳了");
        
        startPosition = transform.position;
        startY = startPosition.y;

        // 获取初始水平前进方向（忽略Y轴，确保水平直线前进）
        horizontalDir = transform.forward;
        horizontalDir.y = 0f;
        if (horizontalDir.magnitude > 0.001f)
            horizontalDir.Normalize();
        else
            horizontalDir = Vector3.forward; // 备用方向

        isJumping = true;
        jumpCoroutine = StartCoroutine(DoParabolicJump());
    }

    private IEnumerator DoParabolicJump()
    {
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration; // 归一化时间 0~1

            // === 1. 计算抛物线位置（整个运动曲线为抛物线）===
            // 水平位移（匀速直线）
            Vector3 horizontalOffset = horizontalDir * (forwardDistance * t);

            // 垂直抛物线位移：y = 4 * jumpHeight * t * (1 - t)
            // 确保起点 y = startY，终点 y = startY，最高点 = startY + jumpHeight
            float yOffset = 4f * jumpHeight * t * (1f - t);
            float newY = startY + yOffset;

            // 组合新位置
            Vector3 newPosition = startPosition + horizontalOffset;
            newPosition.y = newY;

            transform.position = newPosition;

            // === 2. 根据轨迹切线动态修改旋转角度（模拟鲸鱼跳水动态）===
            // 计算当前速度方向（水平速度 + 垂直速度）
            float horizSpeed = forwardDistance / jumpDuration;
            Vector3 horizVel = horizontalDir * horizSpeed;

            // 垂直速度：dy/dt = 4 * jumpHeight * (1 - 2t) / duration
            float vertSpeed = 4f * jumpHeight * (1f - 2f * t) / jumpDuration;
            Vector3 vertVel = Vector3.up * vertSpeed;

            Vector3 velocityDir = horizVel + vertVel;

            // 如果速度不为0，则让鲸鱼朝向运动方向（自然抛物线姿态）
            if (velocityDir.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocityDir.normalized);
                transform.rotation = targetRotation; // 直接对齐（可改成 Quaternion.Slerp 更平滑）
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // === 跳跃结束：恢复到起始Y深度（确保回到水下）===
        Vector3 finalPosition = startPosition + horizontalDir * forwardDistance;
        finalPosition.y = startY;
        transform.position = finalPosition;

        // 可选：跳跃结束后恢复水平姿态（防止最后俯冲角度过大）
        // transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        isJumping = false;
        jumpCoroutine = null;
    }

    private void Update()
    {
        // 测试用：按空格键触发跳跃
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            StartJump();
            Debug.Log("跳");
        }
    }

    // 可选：停止跳跃（如果需要中途打断）
    public void StopJump()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            isJumping = false;
        }
    }
}