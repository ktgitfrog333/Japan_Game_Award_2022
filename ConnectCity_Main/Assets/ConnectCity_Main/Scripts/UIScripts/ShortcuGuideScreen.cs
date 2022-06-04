using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;
using UnityEngine.UI;
using Main.Audio;
using Main.UI;
using Main.Common;

namespace Main.Level
{
    /// <summary>
    /// ショートカット入力
    /// </summary>
    public class ShortcuGuideScreen : MonoBehaviour
    {
        /// <summary>ゲームをやり直すを実行するまで押下し続ける時間</summary>
        [SerializeField] private float undoPushTimeLimit = 3f;
        /// <summary>ステージセレクトへ戻るを実行するまで押下し続ける時間</summary>
        [SerializeField] private float selectPushTimeLimit = 3f;
        /// <summary>遊び方の確認を実行するまで押下し続ける時間</summary>
        [SerializeField] private float manualPushTimeLimit = 3f;
        /// <summary>遊び方の確認のSEパターン</summary>
        [SerializeField] private ClipToPlay manualSEPattern = ClipToPlay.se_play_open_No2;
        /// <summary>リトライのSEパターン</summary>
        [SerializeField] private ClipToPlay retrySEPattern = ClipToPlay.se_retry_No1;
        /// <summary>入力禁止</summary>
        public bool InputBan { get; set; } = false;

        void Start()
        {
            // 各項目を有効にさせるか
            var isPushedContents = new bool[3];
            // ボタン押下
            this.UpdateAsObservable()
                .Where(_ => !InputBan)
                .Subscribe(_ =>
                {
                    if (!CheckAllContentsActive(isPushedContents) && Input.GetButtonDown(InputConst.INPUT_CONSTUNDO))
                        isPushedContents[(int)ShortcuActionMode.UndoAction] = true;
                    else if (!CheckAllContentsActive(isPushedContents) && Input.GetButtonDown(InputConst.INPUT_CONSTSELECT))
                        isPushedContents[(int)ShortcuActionMode.SelectAction] = true;
                    else if (!CheckAllContentsActive(isPushedContents) && Input.GetButtonDown(InputConst.INPUT_CONSTMANUAL))
                        isPushedContents[(int)ShortcuActionMode.CheckAction] = true;
                });
            // ボタンから手を離す
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (isPushedContents[(int)ShortcuActionMode.UndoAction] && Input.GetButtonUp(InputConst.INPUT_CONSTUNDO))
                        isPushedContents[(int)ShortcuActionMode.UndoAction] = false;
                    if (isPushedContents[(int)ShortcuActionMode.SelectAction] && Input.GetButtonUp(InputConst.INPUT_CONSTSELECT))
                        isPushedContents[(int)ShortcuActionMode.SelectAction] = false;
                    if (isPushedContents[(int)ShortcuActionMode.CheckAction] && Input.GetButtonUp(InputConst.INPUT_CONSTMANUAL))
                        isPushedContents[(int)ShortcuActionMode.CheckAction] = false;
                });

            // 押下時間の計測
            var pushingTime = new FloatReactiveProperty();
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (CheckAllContentsActive(isPushedContents))
                    {
                        pushingTime.Value += Time.deltaTime;
                    }
                    else
                        pushingTime.Value = 0f;
                });
            pushingTime.ObserveEveryValueChanged(x => x.Value)
                .Subscribe(x =>
                {
                    if (0f < x)
                    {
                        if (isPushedContents[(int)ShortcuActionMode.UndoAction])
                        {
                            if (1f <= EnabledPushGageAndGetFillAmount(ShortcuActionMode.UndoAction, x, undoPushTimeLimit))
                            {
                                SfxPlay.Instance.PlaySFX(retrySEPattern);
                                SceneOwner.Instance.SetSceneIdUndo();
                                UIOwner.Instance.EnableDrawLoadNowFadeOutTrigger();
                                GameManager.Instance.SpaceOwner.GetComponent<SpaceOwner>().InputBan = true;
                                x = 0f;
                                isPushedContents[(int)ShortcuActionMode.UndoAction] = false;
                            }
                        }
                        else if (isPushedContents[(int)ShortcuActionMode.SelectAction])
                        {
                            if (1f <= EnabledPushGageAndGetFillAmount(ShortcuActionMode.SelectAction, x, selectPushTimeLimit))
                            {
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                SceneOwner.Instance.SetSelectSceneNameIdFromMain_Scene();
                                UIOwner.Instance.EnableDrawLoadNowFadeOutTrigger();
                                GameManager.Instance.SpaceOwner.GetComponent<SpaceOwner>().InputBan = true;
                                x = 0f;
                                isPushedContents[(int)ShortcuActionMode.SelectAction] = false;
                            }
                        }
                        else if (isPushedContents[(int)ShortcuActionMode.CheckAction])
                        {
                            if (1f <= EnabledPushGageAndGetFillAmount(ShortcuActionMode.CheckAction, x, manualPushTimeLimit))
                            {
                                SfxPlay.Instance.PlaySFX(manualSEPattern);
                                UIOwner.Instance.GameManualScrollViewSetActiveFromUIManager(true);
                                x = 0f;
                                isPushedContents[(int)ShortcuActionMode.CheckAction] = false;
                            }
                        }
                    }
                    else
                    {
                        var c = transform.GetChild(0);
                        c.GetChild((int)ShortcuActionMode.UndoAction).GetChild(3).GetComponent<Image>().fillAmount = 0f;
                        c.GetChild((int)ShortcuActionMode.SelectAction).GetChild(3).GetComponent<Image>().fillAmount = 0f;
                        c.GetChild((int)ShortcuActionMode.CheckAction).GetChild(3).GetComponent<Image>().fillAmount = 0f;
                    }
                });
        }

        /// <summary>
        /// 各項目の有効状態をチェック
        /// </summary>
        /// <param name="contents">ゲームをやり直す／ステージセレクトへ戻る／遊び方の確認</param>
        /// <returns>有効／無効</returns>
        private bool CheckAllContentsActive(bool[] contents)
        {
            foreach (var e in contents)
                if (e)
                    return true;
            return false;
        }

        /// <summary>
        /// 対象の項目を有効にして、プッシュゲージを表示する
        /// 渡された押下時間に応じてゲージを変化させる値を返す
        /// </summary>
        /// <param name="mode">各項目のenum</param>
        /// <param name="time">押下時間</param>
        /// <param name="limit">コマンドが実行されるまで押下し続ける時間</param>
        /// <returns>プッシュゲージのfillAmount値</returns>
        private float EnabledPushGageAndGetFillAmount(ShortcuActionMode mode, float time, float limit)
        {
            var content = transform.GetChild(0).GetChild((int)mode);
            var fAmount = time / limit;
            content.GetChild(3).GetComponent<Image>().fillAmount = fAmount;
            return fAmount;
        }
    }

    /// <summary>
    /// ショートカット入力
    /// HierarchyにあるShortcuGuideScreen > BackScreen内のゲームオブジェクトを配列した場合の順番と揃える
    ///     [0]GameUndoLabel    ★
    ///     [1]GameSelectLabel  ★
    ///     [2]GameCheckLabel   ★
    /// </summary>
    public enum ShortcuActionMode
    {
        /// <summary>ステージをやり直す</summary>
        UndoAction
        /// <summary>他のステージを選ぶ</summary>
            , SelectAction
        /// <summary>遊び方の確認</summary>
            , CheckAction
    }
}
