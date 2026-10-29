using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhaleSpawner : MonoBehaviour
{
    [Header("鲸鱼生成设置")]
    [Tooltip("要生成的鲸鱼预制体 (Prefab)")]
    public GameObject whalePrefab;

    [Tooltip("一次生成多少头鲸鱼")]
    public int whaleCount = 10;

    [Tooltip("生成区域的宽度（X轴）")]
    public float spawnWidth = 50f;

    [Tooltip("生成区域的深度（Z轴）")]
    public float spawnDepth = 35f;

    [Tooltip("鲸鱼初始水下深度（Y轴） - 你要求的值")]
    public float underwaterDepth = -10f;

    [Header("跳跃控制")]
    [Tooltip("每头鲸鱼总共跳跃几次（随机 2~5 次）")]
    public int minJumps = 2;
    public int maxJumps = 5;

    [Tooltip("每次跳跃之间的等待时间")]
    public float minJumpInterval = 1.2f;
    public float maxJumpInterval = 3.0f;

    [Tooltip("是否在游戏开始时自动开始")]
    public bool autoStart = true;

    private List<GameObject> spawnedWhales = new List<GameObject>();

    private void Start()
    {
        if (autoStart)
        {
            StartCoroutine(SpawnAndJumpWhales());
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnAndJumpWhales());
    }

    private IEnumerator SpawnAndJumpWhales()
    {
        for (int i = 0; i < whaleCount; i++)
        {
            // 生成位置：Y = -10
            Vector3 spawnPos = new Vector3(
                Random.Range(-spawnWidth / 2f, spawnWidth / 2f),
                underwaterDepth,                    // ← 这里固定为 -10
                Random.Range(-spawnDepth / 2f, spawnDepth / 2f)
            );

            GameObject whale = Instantiate(whalePrefab, spawnPos, Quaternion.identity);
            spawnedWhales.Add(whale);

            // 随机朝向
            float randomYRot = Random.Range(-40f, 40f);
            whale.transform.rotation = Quaternion.Euler(0f, randomYRot, 0f);

            // 获取控制器
            WhaleController controller = whale.GetComponent<WhaleController>();
            if (controller != null)
            {
                // 随机跳跃参数（让每头鲸鱼表现不同）
                controller.jumpDuration = Random.Range(3.8f, 5.2f);
                controller.forwardDistance = Random.Range(14f, 24f);
                controller.jumpHeight = Random.Range(8f, 13f);

                // 让这头鲸鱼执行 2~5 次跳跃
                int jumpTimes = Random.Range(minJumps, maxJumps + 1);
                StartCoroutine(MultiJumpRoutine(controller, jumpTimes, whale));
            }
            else
            {
                Debug.LogWarning("鲸鱼预制体上缺少 WhaleController 组件！");
            }

            // 生成间隔
            yield return new WaitForSeconds(Random.Range(0.4f, 1.1f));
        }
    }

    /// <summary>
    /// 一头鲸鱼多次跳跃的协程（核心功能）
    /// </summary>
    private IEnumerator MultiJumpRoutine(WhaleController controller, int jumpCount, GameObject whaleObject)
    {
        for (int i = 0; i < jumpCount; i++)
        {
            if (controller == null || whaleObject == null) 
                yield break;

            // 执行跳跃
            controller.StartJump();

            // 等待这次跳跃几乎完成后再进行下一次
            // （jumpDuration + 一点额外缓冲时间）
            yield return new WaitForSeconds(controller.jumpDuration + Random.Range(0.6f, 1.4f));
        }

        // 所有跳跃完成后，销毁这头鲸鱼
        if (whaleObject != null)
        {
            Debug.Log($"鲸鱼完成 {jumpCount} 次跳跃，即将销毁");
            Destroy(whaleObject, 0.8f);   // 留一点时间让最后一次入水动作完成
        }
    }

    /// <summary>
    /// 清理所有鲸鱼
    /// </summary>
    public void ClearAllWhales()
    {
        foreach (var whale in spawnedWhales)
        {
            if (whale != null)
                Destroy(whale);
        }
        spawnedWhales.Clear();
    }

    private void OnDestroy()
    {
        ClearAllWhales();
    }
}