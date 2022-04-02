using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;
using Main.UI;

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
        /// <summary>コライダーのサイズ</summary>
        [SerializeField] private Vector3 boxSize;
        /// <summary>コライダーの生成位置</summary>
        [SerializeField] private Vector3 insPosition;

        private void Start()
        {
            var box = GetComponent<BoxCollider>();

            // T.B.D 受け取ったシーン名を元に、コライダーの中央・サイズ、オブジェクト位置を設定
            if (true)
            {
                box.center = new Vector3(0f, 1.7f, 0f);
                box.size = new Vector3(20f, 9.7f, 1f);
                transform.position = Vector3.zero;
            }

            boxCenter = box.center;
            boxSize = box.size;

            this.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER) ||
                    x.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                .Subscribe(_ => {
                    SceneInfoManager.Instance.LoadSceneNameRedo();
                    UIManager.Instance.EnableDrawLoadNowFadeOutTrigger();
                });
        }
    }
}
