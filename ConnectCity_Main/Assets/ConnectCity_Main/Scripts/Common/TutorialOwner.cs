using Main.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Level;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using System.Linq;

namespace Main.Common
{
    /// <summary>
    /// チュートリアルのオーナー
    /// チャンネル0：チュートリアル①_移動
    /// チャンネル1：チュートリアル②_ジャンプ
    /// チャンネル2：チュートリアル③_空間操作
    /// チャンネル3：チュートリアル④_コネクト
    /// チャンネル4：チュートリアル⑤_空間操作_上下左右
    /// チャンネル5：チュートリアル⑥_再利用
    /// </summary>
    public class TutorialOwner : MonoBehaviour, IOwner
    {
        /// <summary>チュートリアルのUI</summary>
        [SerializeField] private GameObject tutorialScreen;
        /// <summary>チュートリアルのエンバイロメント</summary>
        [SerializeField] private GameObject tutorialEnvironment;
        /// <summary>チュートリアルのエンバイロメント</summary>
        public GameObject TutorialEnvironment => tutorialEnvironment;
        /// <summary>ビデオを再生させるトリガー</summary>
        [SerializeField] private GameObject[] triggers;
        /// <summary>コンテンツを表示済みフラグ（ステージ読み込みの度にリセット）</summary>
        private BoolReactiveProperty[] _isPlaiedEvents;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        /// <summary>出現演出の時間</summary>
        [SerializeField] float durationFade = .3f;

        private void Reset()
        {
            if (tutorialScreen == null)
                tutorialScreen = GameObject.Find("TutorialScreen");
            if (tutorialEnvironment == null)
                tutorialEnvironment = GameObject.Find("TutorialEnvironment");
            if (triggers == null || (triggers != null && triggers.Length == 0))
            {
                triggers = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_TUTORIALTRIGGER);
            }
        }

        public bool Initialize()
        {
            try
            {
                if (!tutorialScreen.GetComponent<TutorialScreen>().Initialize())
                    throw new System.Exception("UI初期処理の失敗");
                if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().Initialize())
                    throw new System.Exception("エンバイロメント初期処理の失敗");
                // フラグリセット
                _isPlaiedEvents = ReflashFlags(_isPlaiedEvents);
                foreach (var trigger in triggers)
                {
                    trigger.OnTriggerEnterAsObservable()
                        .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                        .Subscribe(_ =>
                        {
                            var animIdx = (TriggerIndex)GetIdx(trigger.name);
                            switch (animIdx)
                            {
                                case TriggerIndex.Move:
                                    if (!_isPlaiedEvents[(int)animIdx].Value)
                                    {
                                        if (!CloseAny(_isPlaiedEvents))
                                            Debug.LogError("他コンテンツ終了処理の失敗");
                                        if (!tutorialScreen.GetComponent<TutorialScreen>().OpenScreens(ScreenIndex.Move, durationFade))
                                            Debug.LogError("プレイヤーの移動表示処理の失敗");
                                        _isPlaiedEvents[(int)animIdx].Value = true;
                                    }
                                    break;
                                case TriggerIndex.Jump:
                                    if (!_isPlaiedEvents[(int)animIdx].Value)
                                    {
                                        if (!CloseAny(_isPlaiedEvents))
                                            Debug.LogError("他コンテンツ終了処理の失敗");
                                        if (!tutorialScreen.GetComponent<TutorialScreen>().OpenScreens(ScreenIndex.Jump, durationFade))
                                            Debug.LogError("プレイヤーのジャンプ表示処理の失敗");
                                        _isPlaiedEvents[(int)animIdx].Value = true;
                                    }
                                    break;
                                case TriggerIndex.SpaceController:
                                    if (!_isPlaiedEvents[(int)animIdx].Value)
                                    {
                                        if (!CloseAny(_isPlaiedEvents))
                                            Debug.LogError("他コンテンツ終了処理の失敗");
                                        if (!tutorialScreen.GetComponent<TutorialScreen>().OpenScreens(ScreenIndex.Space, durationFade))
                                            Debug.LogError("空間操作の表示処理の失敗");
                                        if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().PlayEnvironment(EnvironmentIndex.SpaceController, durationFade))
                                            Debug.LogError("③_空間操作の表示処理の失敗");
                                        _isPlaiedEvents[(int)animIdx].Value = true;
                                    }
                                    break;
                                case TriggerIndex.SpaceConnect:
                                    if (!_isPlaiedEvents[(int)animIdx].Value)
                                    {
                                        if (!CloseAny(_isPlaiedEvents))
                                            Debug.LogError("他コンテンツ終了処理の失敗");
                                        if (!tutorialScreen.GetComponent<TutorialScreen>().OpenScreens(ScreenIndex.Space, durationFade))
                                            Debug.LogError("空間操作の表示処理の失敗");
                                        if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().PlayEnvironment(EnvironmentIndex.SpaceConnect, durationFade))
                                            Debug.LogError("④_コネクトの表示処理の失敗");
                                        _isPlaiedEvents[(int)animIdx].Value = true;
                                    }
                                    break;
                                case TriggerIndex.SpaceDirection:
                                    if (!_isPlaiedEvents[(int)animIdx].Value)
                                    {
                                        if (!CloseAny(_isPlaiedEvents))
                                            Debug.LogError("他コンテンツ終了処理の失敗");
                                        if (!tutorialScreen.GetComponent<TutorialScreen>().OpenScreens(ScreenIndex.Space, durationFade))
                                            Debug.LogError("空間操作の表示処理の失敗");
                                        if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().PlayEnvironment(EnvironmentIndex.SpaceDirection, durationFade))
                                            Debug.LogError("⑤_空間操作_上下左右の表示処理の失敗");
                                        _isPlaiedEvents[(int)animIdx].Value = true;
                                    }
                                    break;
                                case TriggerIndex.SpaceRecycle:
                                    if (!_isPlaiedEvents[(int)animIdx].Value)
                                    {
                                        if (!CloseAny(_isPlaiedEvents))
                                            Debug.LogError("他コンテンツ終了処理の失敗");
                                        if (!tutorialScreen.GetComponent<TutorialScreen>().OpenScreens(ScreenIndex.Space, durationFade))
                                            Debug.LogError("空間操作の表示処理の失敗");
                                        if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().PlayEnvironment(EnvironmentIndex.SpaceRecycle, durationFade))
                                            Debug.LogError("⑥_再利用の表示処理の失敗");
                                        _isPlaiedEvents[(int)animIdx].Value = true;
                                    }
                                    break;
                                default:
                                    Debug.LogWarning("チュートリアルトリガー処理の例外エラー");
                                    break;
                            }
                        });
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Exit()
        {
            try
            {
                if (!tutorialScreen.GetComponent<TutorialScreen>().Exit())
                    throw new System.Exception("UI終了処理の失敗");
                if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().Exit())
                    throw new System.Exception("エンバイロメント終了処理の失敗");
                _isPlaiedEvents = ReflashFlags(_isPlaiedEvents);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 全てのUIイベントを閉じる
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool CloseEventsAll()
        {
            try
            {
                if (!CloseAny(_isPlaiedEvents))
                    Debug.LogError("他コンテンツ終了処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// フラグリセット
        /// </summary>
        /// <param name="events">イベント実施済みフラグ</param>
        /// <returns>イベントフラグ</returns>
        private BoolReactiveProperty[] ReflashFlags(BoolReactiveProperty[] events)
        {
            events = new BoolReactiveProperty[6];
            for (var i = 0; i < events.Length; i++)
                events[i] = new BoolReactiveProperty();
            return events;
        }

        /// <summary>
        /// 対象以外のチュートリアルを止める
        /// </summary>
        /// <param name="eventsIndex">コンテンツ表示済みフラグ配列</param>
        /// <returns>成功／失敗</returns>
        private bool CloseAny(BoolReactiveProperty[] eventsIndex)
        {
            try
            {
                var targetIdxs = eventsIndex.Select((p, i) => new { Content = p, Index = i })
                    .Where(x => x.Content.Value)
                    .Select(x => x.Index);

                foreach (TriggerIndex idx in targetIdxs)
                    switch (idx)
                    {
                        case TriggerIndex.Move:
                            if (!tutorialScreen.GetComponent<TutorialScreen>().CloseScreens(ScreenIndex.Move, durationFade))
                                Debug.LogError("プレイヤーの移動非表示処理の失敗");
                            break;
                        case TriggerIndex.Jump:
                            if (!tutorialScreen.GetComponent<TutorialScreen>().CloseScreens(ScreenIndex.Jump, durationFade))
                                Debug.LogError("プレイヤーのジャンプ非表示処理の失敗");
                            break;
                        case TriggerIndex.SpaceController:
                            if (!tutorialScreen.GetComponent<TutorialScreen>().CloseScreens(ScreenIndex.Space, durationFade))
                                Debug.LogError("空間操作の非表示処理の失敗");
                            if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().StopEnvironment(EnvironmentIndex.SpaceController, durationFade))
                                Debug.LogError("③_空間操作の非表示処理の失敗");
                            break;
                        case TriggerIndex.SpaceConnect:
                            if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().StopEnvironment(EnvironmentIndex.SpaceConnect, durationFade))
                                Debug.LogError("④_コネクトの非表示処理の失敗");
                            break;
                        case TriggerIndex.SpaceDirection:
                            if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().StopEnvironment(EnvironmentIndex.SpaceDirection, durationFade))
                                Debug.LogError("⑤_空間操作_上下左右の非表示処理の失敗");
                            break;
                        case TriggerIndex.SpaceRecycle:
                            if (!tutorialScreen.GetComponent<TutorialScreen>().CloseScreens(ScreenIndex.Space, durationFade))
                                Debug.LogError("空間操作の非表示処理の失敗");
                            if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().StopEnvironment(EnvironmentIndex.SpaceRecycle, durationFade))
                                Debug.LogError("⑥_再利用の非表示処理の失敗");
                            break;
                        default:
                            Debug.LogWarning("チュートリアルトリガー処理の例外エラー");
                            break;
                    }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 何番目から取得
        /// </summary>
        /// <param name="name">名前（.*_nからnを取得）</param>
        /// <returns>インデックス</returns>
        private int GetIdx(string name)
        {
            return System.Int32.Parse(name.Substring(name.IndexOf("_") + 1));
        }

        public bool ManualStart()
        {
            try
            {
                if (!tutorialEnvironment.GetComponent<TutorialEnvironment>().ManualStart())
                    throw new System.Exception("エンバイロメント疑似スタートの失敗");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }

    /// <summary>
    /// チュートリアルUIのインターフェース
    /// </summary>
    public interface ITutorialOwnerScreen
    {
        /// <summary>
        /// UI表示
        /// </summary>
        /// <param name="index">UI対象番号</param>
        /// <returns>成功／失敗</returns>
        public bool OpenScreens(ScreenIndex index, float durationTime);
        /// <summary>
        /// UIを非表示
        /// </summary>
        /// <param name="index">UI対象番号</param>
        /// <returns>成功／失敗</returns>
        public bool CloseScreens(ScreenIndex index, float durationTime);
    }

    /// <summary>
    /// チュートリアルエンバイロメントのインターフェース
    /// </summary>
    public interface ITutorialOwnerEnvironment
    {
        /// <summary>
        /// ガイドオブジェクト制御を動かす
        /// </summary>
        /// <param name="index">エンバイロメント対象番号</param>
        /// <returns>成功／失敗</returns>
        public bool PlayEnvironment(EnvironmentIndex index, float durationTime);

        /// <summary>
        /// ガイドオブジェクト制御を止める
        /// </summary>
        /// <param name="index">エンバイロメント対象番号</param>
        /// <returns>成功／失敗</returns>
        public bool StopEnvironment(EnvironmentIndex index, float durationTime);
    }

    /// <summary>
    /// トリガーのインデックス
    /// </summary>
    public enum TriggerIndex
    {
        /// <summary>チュートリアル①_移動</summary>
        Move,
        /// <summary>チュートリアル②_ジャンプ</summary>
        Jump,
        /// <summary>チュートリアル③_空間操作</summary>
        SpaceController,
        /// <summary>チュートリアル④_コネクト</summary>
        SpaceConnect,
        /// <summary>チュートリアル⑤_空間操作_上下左右</summary>
        SpaceDirection,
        /// <summary>チュートリアル⑥_再利用</summary>
        SpaceRecycle,
    }

    /// <summary>
    /// スクリーンのインデックス
    /// </summary>
    public enum ScreenIndex
    {
        /// <summary>チュートリアル①_移動</summary>
        Move,
        /// <summary>チュートリアル②_ジャンプ</summary>
        Jump,
        /// <summary>チュートリアル③_空間操作、④～⑥</summary>
        Space,
    }

    /// <summary>
    /// エンバイロメントのインデックス
    /// </summary>
    public enum EnvironmentIndex
    {
        /// <summary>チュートリアル③_空間操作</summary>
        SpaceController,
        /// <summary>チュートリアル④_コネクト</summary>
        SpaceConnect,
        /// <summary>チュートリアル⑤_空間操作_上下左右</summary>
        SpaceDirection,
        /// <summary>チュートリアル⑥_再利用</summary>
        SpaceRecycle,
    }
}
