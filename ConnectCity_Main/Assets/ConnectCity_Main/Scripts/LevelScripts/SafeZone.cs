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
        private void Start()
        {
            var box = GetComponent<BoxCollider>();
            this.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER) ||
                    x.CompareTag(TagConst.TAG_NAME_MOVECUBE))
                .Subscribe(_ => {
                    SceneInfoManager.Instance.SetSceneIdUndo();
                    UIManager.Instance.EnableDrawLoadNowFadeOutTrigger();
                });
        }
    }
}
