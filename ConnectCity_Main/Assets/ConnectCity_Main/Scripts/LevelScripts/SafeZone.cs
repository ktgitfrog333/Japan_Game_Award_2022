using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;
using Main.UI;
using Main.Common;

namespace Main.Level
{
    /// <summary>
    /// セーフゾーン
    /// 範囲外に出ると強制リトライ（やり直し）
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class SafeZone : MonoBehaviour
    {
        /// <summary>コライダーの中央</summary>
        [SerializeField] private Vector3 boxCenter;
        /// <summary>コライダーの中央</summary>
        public Vector3 BoxCenter
        {
            set
            {
                boxCenter = value;
            }
        }
        /// <summary>コライダーのサイズ</summary>
        [SerializeField] private Vector3 boxSize;
        /// <summary>コライダーのサイズ</summary>
        public Vector3 BoxSize
        {
            set
            {
                boxSize = value;
            }
        }
        /// <summary>コライダーの生成位置</summary>
        [SerializeField] private Vector3 insPosition;
        /// <summary>コライダーの生成位置</summary>
        public Vector3 InsPosition
        {
            set
            {
                insPosition = value;
            }
        }

        private void Start()
        {
            var box = GetComponent<BoxCollider>();

            // T.B.D 受け取ったシーン名を元に、コライダーの中央・サイズ、オブジェクト位置を設定
            if (true)
            {
                box.center = boxCenter;
                box.size = boxSize;
                transform.localPosition = insPosition;
            }

            this.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER) ||
                    x.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                .Subscribe(_ => {
                    SceneInfoManager.Instance.SetSceneIdRedo();
                    UIManager.Instance.EnableDrawLoadNowFadeOutTrigger();
                });
        }
    }
}
