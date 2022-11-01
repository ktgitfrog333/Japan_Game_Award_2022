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
    public class GoalPoint : MonoBehaviour, IOwner
    {
        /// <summary>コネクト残り回数のカウントダウンスクリーンプレハブ</summary>
        [SerializeField] private GameObject connectCountScreenPrefab;
        /// <summary>カウントダウンのUI</summary>
        private GameObject _connectCountScreen;
        /// <summary>接地判定用のレイ　オブジェクトの始点</summary>
        private static readonly Vector3 ISGROUNDED_RAY_ORIGIN_OFFSET = new Vector3(0f, 0.1f);
        /// <summary>接地判定用のレイ　オブジェクトの終点</summary>
        private static readonly Vector3 ISGROUNDED_RAY_DIRECTION = Vector3.down;
        /// <summary>接地判定用のレイ　当たり判定の最大距離</summary>
        private static readonly float ISGROUNDED_RAY_MAX_DISTANCE = .25f;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

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
                screen.GetComponent<ConnectCountScreen>().Initialize(transform, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.GetComponent<Camera>());
                _connectCountScreen = screen;
            }
            else
                _connectCountScreen.GetComponent<ConnectCountScreen>().Initialize(transform, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.GetComponent<Camera>());

            if (!UpdateCountDown(0, GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().ClearConnectedCounter))
                Debug.LogError("カウンター初期値セットの失敗");
            if (GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().ClearConnectedCounter == 0)
            {
                if (!OpenDoor())
                    Debug.LogError("ドアを開ける処理の失敗");
            }
            else
            {
                if (!CloseDoor())
                    Debug.LogError("ドアを閉める処理の失敗");
            }

            var rOrgOffAry = LevelDesisionIsObjected.GetTwoPointHorizontal(ISGROUNDED_RAY_ORIGIN_OFFSET, .5f);
            this.FixedUpdateAsObservable()
                .Where(_ => !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[0], ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[1], ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[0], ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)) &&
                    !LevelDesisionIsObjected.IsOnPlayeredAndInfo(transform.position, rOrgOffAry[1], ISGROUNDED_RAY_DIRECTION, ISGROUNDED_RAY_MAX_DISTANCE, LayerMask.GetMask(LayerConst.LAYER_NAME_MOVECUBE)))
                .Subscribe(_ =>
                {
                    transform.position += Physics.gravity * Time.deltaTime;
                    if (_connectCountScreen != null)
                        _connectCountScreen.GetComponent<ConnectCountScreen>().Initialize(transform, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.GetComponent<Camera>());
                })
                .AddTo(_compositeDisposable);

            return true;
        }

        /// <summary>
        /// カウントダウン表示を更新
        /// </summary>
        /// <param name="count">コネクト回数</param>
        /// <param name="maxCount">クリア条件のコネクト必要回数</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDown(int count, int maxCount)
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
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool OpenDoor()
        {
            try
            {
                // 扉を開くアニメーションを再生
                transform.GetChild(1).GetComponent<Animation>().Play("open");

                var disposable = new SingleAssignmentDisposable();
                var complete = false;
                transform.GetChild(0).GetComponent<SphereCollider>().enabled = true;
                // プレイヤーオブジェクトがゴールに触れる
                disposable.Disposable = transform.GetChild(0).OnTriggerEnterAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER) && !complete)
                    .Subscribe(_ =>
                    {
                        complete = true;
                        if (!PlayClearDirectionAndOpenClearScreen())
                            Debug.LogWarning("ゴール演出エラー発生");
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
        /// LevelOwnerからの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool CloseDoorFromLevelOwner()
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
                // 扉を閉めるアニメーションを再生
                transform.GetChild(1).GetComponent<Animation>().Play("close");
                transform.GetChild(0).GetComponent<SphereCollider>().enabled = false;
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
                if (!GameManager.Instance.TutorialOwner.GetComponent<TutorialOwner>().CloseEventsAll())
                    Debug.LogError("チュートリアルのUIイベントリセット処理の失敗");
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetPlayerControllerInputBan(true);
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().ChangeCharactorControllerStatePlayer(false))
                    Debug.LogError("プレイヤーのCharactorControllerステータス変更の失敗");
                // 空間操作を禁止
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetSpaceOwnerInputBan(true);
                // ショートカット入力を禁止
                GameManager.Instance.UIOwner.GetComponent<UIOwner>().SetShortcuGuideScreenInputBan(true);
                var complete = GameManager.Instance.UIOwner.GetComponent<UIOwner>().PlayEndCutsceneFromGoalPoint();
                complete.ObserveEveryValueChanged(x => x.Value)
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlaySFX(ClipToPlay.me_game_clear);
                        GameManager.Instance.UIOwner.GetComponent<UIOwner>().OpenClearScreen();
                    });

                return true;
            }
            else
            {
                Debug.LogWarning("ゴール下に足場がありません");
                return false;
            }
        }

        public bool ManualStart()
        {
            throw new System.NotImplementedException();
        }

        public bool Exit()
        {
            try
            {
                _compositeDisposable.Clear();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}
