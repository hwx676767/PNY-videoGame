using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDector : MonoBehaviour
{
    public bool isDector = false;
    public bool canCollect = false;

    public GameObject collectUI;    //一直不变的ui
    private GameObject currentUiObject;     //每个物体特定的
    private GameObject currentCollectObject;

    private void Start()
    {
        collectUI.SetActive(false);
    }

    private void Update()
    {
        if (isDector)
        {
            CatchPlayer();
            Debug.Log("发现玩家，追逐");
        }
        else
        {
            NPCMove();
        }

        NPCCollected();

    }

    public void NPCMove()
    {
        //寻路移动 、固定范围移动
    }

    public void CatchPlayer()
    {
        //追着玩家走
        //npc的移动方向 == 玩家的位置  player.position
    }

    public void NPCCollected()
    {
        if (canCollect)
        {
            collectUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                //拾取物品
                Destroy(currentCollectObject);
                //ui更新 -- 收集物数目 ++  弹出ui-你收集了XXX
            }
        }
        else
        {
            collectUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("检测到玩家进入");
            isDector = true;
        }

        if (other.CompareTag("collectable"))
        {
            canCollect = true;
            currentCollectObject = other.gameObject;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("检测到玩家退出");
            isDector = false;
        }
        
        if (other.CompareTag("collectable"))
        {
            canCollect = false;
        }
    }
}
