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

            if (!UpdateCountDown(0, SceneInfoManager.Instance.ClearConnectedCounter))
                Debug.LogError("カウンター初期値セットの失敗");
            if (SceneInfoManager.Instance.ClearConnectedCounter == 0)
            {
                if (!OpenDoor())
                    Debug.LogError("ドアを開ける処理の失敗");
            }
            else
            {
                if (!CloseDoor())
                    Debug.LogError("ドアを閉める処理の失敗");
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
            return UpdateCountDown(count, maxCount);
        }


        /// <summary>
        /// カウントダウン表示を更新
        /// </summary>
        /// <param name="count">コネクト回数</param>
        /// <param name="maxCount">クリア条件のコネクト必要回数</param>
        /// <returns>成功／失敗</returns>
        private bool UpdateCountDown(int count, int maxCount)
        {
            try
            {
                // カウントダウン結果を格納
                var result = maxCount - count;
                // カウントがゼロなら顔文字「^^」へ表示を更新
                _connectCountScreen.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = 0 < result ? result + "" : "";
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
            return OpenDoor();
        }

        /// <summary>
        /// ドアを開く
        /// ゴール演出のイベント
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool OpenDoor()
        {
            try
            {
                // 扉を開くアニメーションを再生
                transform.GetChild(1).GetComponent<Animation>().Play("open");

                var disposable = new SingleAssignmentDisposable();
                var complete = false;
                // プレイヤーオブジェクトがゴールに触れる
                disposable.Disposable = transform.GetChild(0).OnTriggerEnterAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER) && !complete)
                    .Subscribe(_ =>
                    {
                        complete = true;
                        if (!PlayClearDirectionAndOpenClearScreen())
                            Debug.Log("ゴール演出エラー発生");
                        disposable.Dispose();
                    });

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
            return CloseDoor();
        }

        /// <summary>
        /// ドアを閉じる
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool CloseDoor()
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
                if (!GameManager.Instance.SetBanPlayerFromGoalPoint(true))
                    Debug.LogError("プレイヤー操作禁止フラグ切り替え処理の失敗");
                // 空間操作を禁止
                GameManager.Instance.SpaceManager.GetComponent<SpaceManager>().InputBan = true;
                var complete = UIManager.Instance.PlayEndCutsceneFromGoalPoint();
                complete.ObserveEveryValueChanged(x => x.Value)
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        SfxPlay.Instance.PlaySFX(ClipToPlay.me_game_clear);
                        UIManager.Instance.OpenClearScreen();
                    });

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
