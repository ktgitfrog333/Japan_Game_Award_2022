using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;
using Main.UI;
using Main.Common;
using Main.Audio;

namespace Main.Level
{
    /// <summary>
    /// セーフゾーン
    /// 範囲外に出ると強制リトライ（やり直し）
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class SafeZone : MonoBehaviour
    {
        /// <summary>落下のSEパターン</summary>
        [SerializeField] private ClipToPlay fallSEPattern = ClipToPlay.se_player_fall_No1;

        private void Start()
        {
            var box = GetComponent<BoxCollider>();
            this.OnTriggerExitAsObservable()
                .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                .Subscribe(_ => {
                    GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(fallSEPattern);
                    GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SetSceneIdUndo();
                    if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetWarpGatesPairInputBan(true))
                        Debug.LogError("フラグ切り替え処理の失敗");
                    if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().KillCompleteTweeenSuction())
                        Debug.LogError("TweenアニメーションKill呼び出しの失敗");
                    GameManager.Instance.UIOwner.GetComponent<UIOwner>().EnableDrawLoadNowFadeOutTrigger();
                });
        }
    }
}
