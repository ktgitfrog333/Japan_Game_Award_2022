using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;
using Main.Common;
using Main.InputSystem;
using Main.Level;
using Main.Common.LevelDesign;

namespace Main.UI
{
    /// <summary>
    /// チュートリアル画面
    /// トリガーとなるオブジェクト（TutorialTrigger_0）
    /// ※上記オブジェクトを「*_0...99」のように一意の番号で付けることでビデオクリップの配列番号「channels」と連動する
    /// </summary>
    public class TutorialScreen : MonoBehaviour, IOwner, ITutorialOwnerScreen
    {
        /// <summary>左空間の色</summary>
        [SerializeField] private Color keyLeftDefaultColor = new Color(47f, 148f, 180f, 255f);
        /// <summary>右空間の色</summary>
        [SerializeField] private Color keyRightDefaultColor = new Color(180f, 54f, 47f, 255f);
        /// <summary>左空間の色</summary>
        [SerializeField] private Color keyEnabledLeftColor = new Color(47f, 148f, 180f, 255f);
        /// <summary>右空間の色</summary>
        [SerializeField] private Color keyEnabledRightColor = new Color(180f, 54f, 47f, 255f);
        /// <summary>Transformキャッシュ</summary>
        private Transform _transform;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public bool ManualStart()
        {
            try
            {
                _transform = transform;
                // 初期状態はガイドを表示させない
                foreach (Transform g in _transform)
                    g.gameObject.SetActive(false);

                var inputMode = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().CurrentInputMode;
                var isOpenedSpace = new BoolReactiveProperty();
                isOpenedSpace.Value = _transform.GetChild((int)ScreenIndex.Space).gameObject.activeSelf;
                _transform.GetChild((int)ScreenIndex.Space).gameObject.OnEnableAsObservable()
                    .Subscribe(x => isOpenedSpace.Value = true)
                    .AddTo(_compositeDisposable);
                _transform.GetChild((int)ScreenIndex.Space).gameObject.OnDisableAsObservable()
                    .Subscribe(x => isOpenedSpace.Value = false)
                    .AddTo(_compositeDisposable);
                // 空間操作UIのアクティブ状態を監視
                isOpenedSpace.ObserveEveryValueChanged(x => x.Value)
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        if (!ChangeSpaceGuide((InputMode)inputMode.Value))
                            throw new System.Exception("空間操作のUI切り替えの失敗");
                    })
                    .AddTo(_compositeDisposable);
                // 入力インタフェースによって表示するUIを切り替える
                inputMode.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        if (isOpenedSpace.Value)
                            if (!ChangeSpaceGuide((InputMode)x))
                                throw new System.Exception("空間操作のUI切り替えの失敗");
                    })
                    .AddTo(_compositeDisposable);
                // コントローラーのUI
                var leftTran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Gamepad)
                                    .GetChild((int)Direction2D.Left).GetChild(1);
                var leftPos = leftTran.localPosition;
                var rightTran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Gamepad)
                                    .GetChild((int)Direction2D.Right).GetChild(1);
                var rightPos = rightTran.localPosition;
                // キーボードのUI（左）
                var key2Tran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                    .GetChild(0);
                var leftbaseTran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                    .GetChild(1);
                var qKey = leftbaseTran.GetChild(0);
                var wKey = leftbaseTran.GetChild(1);
                var eKey = leftbaseTran.GetChild(2);
                var rKey = leftbaseTran.GetChild(4);
                // キーボードのUI（右）
                var key9Tran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                    .GetChild(2);
                var rightbaseTran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                    .GetChild(3);
                var uKey = rightbaseTran.GetChild(0);
                var iKey = rightbaseTran.GetChild(2);
                var oKey = rightbaseTran.GetChild(3);
                var pKey = rightbaseTran.GetChild(4);

                var environment = GameManager.Instance.TutorialOwner.GetComponent<TutorialOwner>().TutorialEnvironment.GetComponent<TutorialEnvironment>();

                this.FixedUpdateAsObservable()
                    .Where(_ => isOpenedSpace.Value)
                    .Subscribe(_ =>
                    {
                        Vector3[] inputs = environment.CurrentDirection;

                        switch ((InputMode)inputMode.Value)
                        {
                            case InputMode.Gamepad:
                                leftTran.DOLocalMove(leftPos + inputs[(int)Direction2D.Left] * 60f, .1f);
                                rightTran.DOLocalMove(rightPos + inputs[(int)Direction2D.Right] * 60f, .1f);
                                break;
                            case InputMode.Keyboard:
                                // 左空間操作中なら確定で光らせる
                                if (0f < inputs[(int)Direction2D.Left].magnitude)
                                    PlayAndReturnDOColor(rKey, keyEnabledLeftColor, keyLeftDefaultColor);
                                // 左空間の左操作
                                if (inputs[(int)Direction2D.Left].x < 0f)
                                    PlayAndReturnDOColor(qKey, keyEnabledLeftColor, keyLeftDefaultColor);
                                // 左空間の右操作
                                if (0f < inputs[(int)Direction2D.Left].x)
                                    PlayAndReturnDOColor(eKey, keyEnabledLeftColor, keyLeftDefaultColor);
                                // 左空間の下操作
                                if (inputs[(int)Direction2D.Left].y < 0f)
                                    PlayAndReturnDOColor(wKey, keyEnabledLeftColor, keyLeftDefaultColor);
                                // 左空間の上操作
                                if (0f < inputs[(int)Direction2D.Left].y)
                                    PlayAndReturnDOColor(key2Tran, keyEnabledLeftColor, keyLeftDefaultColor);

                                // 右空間操作中なら確定で光らせる
                                if (0f < inputs[(int)Direction2D.Right].magnitude)
                                    PlayAndReturnDOColor(uKey, keyEnabledRightColor, keyRightDefaultColor);
                                // 右空間の左操作
                                if (inputs[(int)Direction2D.Right].x < 0f)
                                    PlayAndReturnDOColor(iKey, keyEnabledRightColor, keyRightDefaultColor);
                                // 右空間の右操作
                                if (0f < inputs[(int)Direction2D.Right].x)
                                    PlayAndReturnDOColor(pKey, keyEnabledRightColor, keyRightDefaultColor);
                                // 右空間の下操作
                                if (inputs[(int)Direction2D.Right].y < 0f)
                                    PlayAndReturnDOColor(oKey, keyEnabledRightColor, keyRightDefaultColor);
                                // 右空間の上操作
                                if (0f < inputs[(int)Direction2D.Right].y)
                                    PlayAndReturnDOColor(key9Tran, keyEnabledRightColor, keyRightDefaultColor);

                                break;
                            default:
                                throw new System.Exception("入力モード判定の例外");
                        }
                    })
                    .AddTo(_compositeDisposable);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// DOColorの拡張版
        /// 完了後に元の色へ戻す
        /// </summary>
        /// <param name="key">キーボードのキーUI</param>
        /// <param name="enabledColor">有効状態の色</param>
        /// <param name="defaultColor">デフォルトカラー</param>
        private void PlayAndReturnDOColor(Transform key, Color enabledColor, Color defaultColor)
        {
            key.GetComponent<Image>().DOColor(enabledColor, .1f)
                .OnComplete(() => key.GetComponent<Image>().color = defaultColor);
        }

        /// <summary>
        /// 空間操作のUI切り替え
        /// </summary>
        /// <param name="mode">入力モード</param>
        /// <returns>成功／失敗</returns>
        private bool ChangeSpaceGuide(InputMode mode)
        {
            try
            {
                var current = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)mode);
                if (!current.gameObject.activeSelf)
                    current.gameObject.SetActive(true);
                switch (mode)
                {
                    case InputMode.Gamepad:
                        // コントローラーUI以外は閉じる
                        var key = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard);
                        if (key.gameObject.activeSelf)
                            key.gameObject.SetActive(false);
                        break;
                    case InputMode.Keyboard:
                        // キーボードのUI
                        var key2Tran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                            .GetChild(0);
                        var leftbaseTran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                            .GetChild(1);
                        var qKey = leftbaseTran.GetChild(0);
                        var wKey = leftbaseTran.GetChild(1);
                        var eKey = leftbaseTran.GetChild(2);
                        var rKey = leftbaseTran.GetChild(4);

                        var key9Tran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                            .GetChild(2);
                        var rightbaseTran = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Keyboard)
                            .GetChild(3);
                        var uKey = rightbaseTran.GetChild(0);
                        var iKey = rightbaseTran.GetChild(2);
                        var oKey = rightbaseTran.GetChild(3);
                        var pKey = rightbaseTran.GetChild(4);

                        // デフォルトカラー設定
                        key2Tran.GetComponent<Image>().color = keyLeftDefaultColor;
                        qKey.GetComponent<Image>().color = keyLeftDefaultColor;
                        wKey.GetComponent<Image>().color = keyLeftDefaultColor;
                        eKey.GetComponent<Image>().color = keyLeftDefaultColor;
                        rKey.GetComponent<Image>().color = keyLeftDefaultColor;

                        key9Tran.GetComponent<Image>().color = keyRightDefaultColor;
                        uKey.GetComponent<Image>().color = keyRightDefaultColor;
                        iKey.GetComponent<Image>().color = keyRightDefaultColor;
                        oKey.GetComponent<Image>().color = keyRightDefaultColor;
                        pKey.GetComponent<Image>().color = keyRightDefaultColor;

                        // キーボードUI以外は閉じる
                        var gpad = _transform.GetChild((int)ScreenIndex.Space).GetChild((int)InputMode.Gamepad);
                        if (gpad.gameObject.activeSelf)
                            gpad.gameObject.SetActive(false);
                        break;
                    default:
                        throw new System.Exception("入力モード判定の例外");
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool OpenScreens(ScreenIndex index, float durationTime)
        {
            var screen = _transform.GetChild((int)index);
            screen.gameObject.SetActive(true);
            var group = screen.GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.DOFade(endValue: 1f, duration: durationTime);

            return true;
        }

        public bool CloseScreens(ScreenIndex index, float durationTime)
        {
            var screen = _transform.GetChild((int)index);
            var group = screen.GetComponent<CanvasGroup>();
            group.DOFade(endValue: 0f, duration: durationTime)
                .OnComplete(() => screen.gameObject.SetActive(false))
                .SetLink(gameObject);

            return true;
        }

        public bool Exit()
        {
            try
            {
                // 各UIを無効
                foreach (Transform g in _transform)
                    g.gameObject.SetActive(false);
                _compositeDisposable.Clear();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
