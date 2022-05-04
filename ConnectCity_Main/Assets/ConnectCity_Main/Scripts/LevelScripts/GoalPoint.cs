using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using Main.Common.LevelDesign;
using Main.Audio;
using Main.UI;
using Main.Common;
using UnityEngine.UI;

namespace Main.Level
{
    /// <summary>
    /// ゴールエリア
    /// </summary>
    public class GoalPoint : MonoBehaviour
    {
        /// <summary>コネクト残り回数のカウントダウンスクリーンプレハブ</summary>
        [SerializeField] private GameObject connectCountScreenPrefab;
        private GameObject _connectCountScreen;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        private static readonly Vector3 ISGROUNDED_RAY_ORIGIN_OFFSET = new Vector3(0f, 0.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        private static readonly Vector3 ISGROUNDED_RAY_DIRECTION = Vector3.down;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        private static readonly float ISGROUNDED_RAY_MAX_DISTANCE = 1.5f;

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool Initialize()
        {
            if (_connectCountScreen == null)
            {
                GameObject screen = GameObject.FindGameObjectWithTag(TagConst.TAG_NAME_CONNECTCOUNTSCREEN);
                if (screen == null)
                {
                    // コネクト回数カウントダウンUIを生成
                    screen = Instantiate(connectCountScreenPrefab);
                }
                screen.GetComponent<ConnectCountScreen>().Initialize(transform, GameManager.Instance.MainCamera.GetComponent<Camera>());
                _connectCountScreen = screen;
            }
            return true;
        }

        /// <summary>
        /// カウントダウン表示を更新
        /// GameManagerからの呼び出し
        /// </summary>
        /// <param name="count">コネクト回数</param>
        /// <param name="maxCount">クリア条件のコネクト必要回数</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDownFromGameManager(int count, int maxCount)
        {
            try
            {
                // カウントダウン結果を格納
                var result = maxCount - count;
                // カウントがゼロなら顔文字「^^」へ表示を更新
                _connectCountScreen.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = 0 < result ? result + "" : "^^";
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ドアを開く
        /// ゴール演出のイベント
        /// GameManagerからの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool OpenDoorFromGameManager()
        {
            try
            {
                // 扉を開くアニメーションを再生
                transform.GetChild(1).GetComponent<Animation>().Play("open");

                // プレイヤーオブジェクトがゴールに触れる
                transform.GetChild(0).OnTriggerEnterAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                    .Select(_ => PlayClearDirectionAndOpenClearScreen())
                    .Where(x => !x)
                    .Subscribe(_ => Debug.Log("ゴール演出エラー発生"));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ドアを閉じる
        /// GameManagerからの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool CloseDoorFromGameManager()
        {
            try
            {
                // 扉を開くアニメーションを再生
                transform.GetChild(1).GetComponent<Animation>().Play("close");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// クリア演出の再生、クリア画面の表示
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool PlayClearDirectionAndOpenClearScreen()
        {
            if (LevelDesisionIsObjected.IsGrounded(transform.position, ISGROUNDED_RAY_ORIGIN_OFFSET, ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE))
            {
                // T.B.D プレイヤー操作を停止する処理を追加
                // T.B.D ゴール演出を入れるなら追加
                SfxPlay.Instance.PlaySFX(ClipToPlay.me_game_clear);
                UIManager.Instance.OpenClearScreen();

                return true;
            }
            else
            {
                Debug.Log("ゴール下に足場がありません");
                return false;
            }
        }
    }
}
