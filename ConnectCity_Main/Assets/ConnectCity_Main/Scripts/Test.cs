using Main.Common;
using Main.Level;
using System.Collections;
using System.Collections.Generic;
using TitleSelect;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private bool flag;
    private bool singleOpe;

    private void Start()
    {
        Debug.Log(BrideScenes_SelectMain.Instance.LoadSceneId);
    }

    private void Update()
    {
        if (flag)
        {
            if (!singleOpe)
            {
                singleOpe = true;
                BrideScenes_SelectMain.Instance.SetMainSceneNameIdFromSelect_Scene(0);
                BrideScenes_SelectMain.Instance.PlayLoadScene();
            }
        }
    }

    //[SerializeField] private bool gravityControllerActive;
    //[SerializeField] private Direction direction;
    //[SerializeField, Range(0.1f, 8f)] private float gravity = .1f;

    //private void FixedUpdate()
    //{
    //    // 重力操作ギミックのデモ制御
    //    if (gravityControllerActive)
    //    {
    //        switch (direction)
    //        {
    //            case Direction.LEFT:
    //                if (!GameManager.Instance.MoveCharactorFromGravityController(Vector3.left * gravity * (1f - Time.deltaTime)))
    //                    Debug.LogError("プレイヤー操作の失敗");
    //                break;
    //            case Direction.RIGHT:
    //                if (!GameManager.Instance.MoveCharactorFromGravityController(Vector3.right * gravity * (1f - Time.deltaTime)))
    //                    Debug.LogError("プレイヤー操作の失敗");
    //                break;
    //            case Direction.UP:
    //                if (!GameManager.Instance.MoveCharactorFromGravityController(Vector3.up * gravity * (1f - Time.deltaTime)))
    //                    Debug.LogError("プレイヤー操作の失敗");
    //                break;
    //            case Direction.DOWN:
    //                if (!GameManager.Instance.MoveCharactorFromGravityController(Vector3.down * gravity * (1f - Time.deltaTime)))
    //                    Debug.LogError("プレイヤー操作の失敗");
    //                break;
    //            default:
    //                Debug.LogError("指定無し");
    //                break;
    //        }
    //    }
    //}
}
