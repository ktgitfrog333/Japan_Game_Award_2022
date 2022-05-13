using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gimmick
{
    /// <summary>
    /// 重力操作ギミック
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ChangeGravity : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3; //移動速度
        [SerializeField] private Vector3 localGravity;
        [SerializeField] private Direction direction;
        float moveX = 0f;
        float moveZ = 0f;

        // Start is called before the first frame update
        void Start()
        {
            localGravity = transform.forward;
        }

        // Update is called once per frame
        void Update()
        {
            switch (direction)
            {
                case Direction.UP:
                    // 重力を上向きにセット
                    localGravity = Vector3.up * moveSpeed;
                    break;
                case Direction.DOWN:
                    // 重力を下向きにセット
                    localGravity = Vector3.down * moveSpeed;
                    break;
                case Direction.LEFT:
                    // 重力を左向きにセット
                    localGravity = Vector3.left * moveSpeed;
                    break;
                case Direction.RIGHT:
                    // 重力を右向きにセット
                    localGravity = Vector3.right * moveSpeed;
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            localGravity = Vector3.zero;
        }

    }

    public enum Direction
    {
        /// <summary>上</summary>
        UP
    /// <summary>下</summary>
    , DOWN
    /// <summary>左</summary>
    , LEFT
    /// <summary>右</summary>
    , RIGHT
    }
}
