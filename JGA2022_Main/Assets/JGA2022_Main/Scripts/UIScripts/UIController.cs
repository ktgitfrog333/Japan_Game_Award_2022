using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using System.Threading.Tasks;

/// <summary>
/// UI操作クラス
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(ActionMode))]
[RequireComponent(typeof(Image))]
public class UIController : MonoBehaviour
{
    /// <summary>選択状態フレーム</summary>
    [SerializeField] private GameObject frame;
    /// <summary>アクションモード</summary>
    [SerializeField] private ActionMode act;
    /// <summary>UIの共通制御</summary>
    private UIControllerCommon _common;

    /// <summary>メニューを閉じる際に一度のみ実行するよう制御するフラグ</summary>
    private bool _menuClose;
    /// <summary>ボタン</summary>
    [SerializeField] private Button button;
    /// <summary>イベントシステム</summary>
    [SerializeField] private EventTrigger eventTrigger;
    /// <summary>ボタンの画像</summary>
    [SerializeField] private Image image;

    private void Awake()
    {
        // ポーズ画面のStartイベントより参照があるため、事前にセットさせる
        if (_common == null)
        {
            _common = new UIControllerCommon(transform);
        }
        EntryEventTrigger();
    }

    private void OnEnable()
    {
        if (!act)
            act = GetComponent<ActionMode>();
        _menuClose = false;
        if (button.isActiveAndEnabled == false)
        {
            button.enabled = true;
        }
    }

    private void Reset()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    private void Initialize()
    {
        if (!act)
            act = GetComponent<ActionMode>();

        // 共通処理
        if (!frame)
        {
            frame = transform.GetChild(0).gameObject;
            frame.SetActive(false);
        }

        // 各項目によって変わる
        switch (act.mode)
        {
            case PauseActionMode.BackAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)PauseActionMode.CheckAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)PauseActionMode.RedoAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            case PauseActionMode.RedoAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)PauseActionMode.BackAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)PauseActionMode.SelectAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            case PauseActionMode.SelectAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)PauseActionMode.RedoAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)PauseActionMode.CheckAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            case PauseActionMode.CheckAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)PauseActionMode.SelectAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)PauseActionMode.BackAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            default:
                Debug.Log("アクションモード未設定");
                Debug.Log("オブジェクト名:[" + name + "]");
                break;
        }
        if (!image)
            image = GetComponent<Image>();
    }

    /// <summary>
    /// イベントトリガーを設定する
    /// </summary>
    private void EntryEventTrigger()
    {
        if (!eventTrigger)
            eventTrigger = GetComponent<EventTrigger>();
        var entryAry = new List<EventTrigger.Entry>();
        entryAry.Add(GetEntryEvent(EventTriggerType.Select, (data) => Selected()));
        entryAry.Add(GetEntryEvent(EventTriggerType.Deselect, (data) => Deselected()));
        entryAry.Add(GetEntryEvent(EventTriggerType.Submit, (data) => Submited()));
        entryAry.Add(GetEntryEvent(EventTriggerType.Cancel, (data) => Canceled()));
        eventTrigger.triggers = entryAry;
    }

    /// <summary>
    /// イベントトリガーのエントリー要素としてIDとイベントリスナー登録を実施して取得する
    /// </summary>
    /// <param name="type">イベントトリガータイプ</param>
    /// <param name="action">登録したいイベント関数</param>
    /// <returns>イベントトリガーのエントリー要素</returns>
    private EventTrigger.Entry GetEntryEvent(EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var e = new EventTrigger.Entry();
        e.eventID = type;
        e.callback.AddListener(action);

        return e;
    }

    /// <summary>
    /// 選択時に呼び出すメソッド
    /// </summary>
    public void Selected()
    {
        _common.SelectContent();
        frame.SetActive(true);
        SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
    }

    /// <summary>
    /// 選択解除時に呼び出すメソッド
    /// </summary>
    public void Deselected()
    {
        frame.SetActive(false);
        _common.DeSelectContent();
    }

    /// <summary>
    /// 選択項目の決定時に呼び出すメソッド
    /// </summary>
    public void Submited()
    {
        // 各項目によって変わる
        switch (act.mode)
        {
            case PauseActionMode.BackAction:
                if (_menuClose == false)
                {
                    _menuClose = true;
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_close);
                    UIManager.Instance.CloseMenu();
                    button.enabled = false;
                    // T.B.D　ボタン押下時の演出を追加
                    PlayFlashingMotion();
                }
                break;
            case PauseActionMode.RedoAction:
                if (_menuClose == false)
                {
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                    SceneInfoManager.Instance.LoadSceneNameRedo();
                    LoadNow.Instance.gameObject.SetActive(true);
                    LoadNow.Instance.DrawLoadNowFadeOutTrigger = true;

                    _menuClose = true;
                    button.enabled = false;
                    PlayFlashingMotion();
                }
                break;
            case PauseActionMode.CheckAction:
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                GameManualScrollView.Instance.gameObject.SetActive(true);
                break;
            case PauseActionMode.SelectAction:
                if (_menuClose == false)
                {
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                    SceneInfoManager.Instance.LoadSceneNameSelect();
                    LoadNow.Instance.DrawLoadNowFadeOutTrigger = true;
                    _menuClose = true;
                    button.enabled = false;
                    PlayFlashingMotion();
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 点滅演出
    /// </summary>
    private async void PlayFlashingMotion()
    {
        // 点滅の演出
        await Task.Delay(80);
        image.DOColor(Color.gray, .3f);
        await Task.Delay(80);
        image.DOColor(Color.white, .3f);
    }

    /// <summary>
    /// キャンセル時に呼び出すメソッド
    /// </summary>
    public void Canceled()
    {
        if (_menuClose == false)
        {
            frame.SetActive(false);
            _common.DeSelectContent();

            _menuClose = true;
            SfxPlay.Instance.PlaySFX(ClipToPlay.se_close);
            UIManager.Instance.CloseMenu();
            button.enabled = false;
            // T.B.D　ボタン押下時の演出を追加
        }
    }
}

/// <summary>
/// UIの共通制御
/// </summary>
public class UIControllerCommon
{
    /// <summary>位置・回転・スケール情報</summary>
    public RectTransform RTransform { get; private set; }
    /// <summary>幅</summary>
    private float _width;
    /// <summary>高さ</summary>
    private float _height;
    /// <summary>一番最初のみ実施</summary>
    private bool _first = true;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="transform">項目のトランスフォーム情報</param>
    public UIControllerCommon(Transform transform)
    {
        this.RTransform = transform as RectTransform;
    }

    /// <summary>
    /// 選択状態の演出
    /// </summary>
    public void SelectContent()
    {
        if (_first == true)
        {
            _width = RTransform.sizeDelta.x;
            _height = RTransform.sizeDelta.y;

            _first = false;
        }
        RTransform.sizeDelta = new Vector2(_width * 1.2f, _height * 1.2f);
    }

    /// <summary>
    /// 選択解除の演出
    /// </summary>
    public void DeSelectContent()
    {
        RTransform.sizeDelta = new Vector2(_width, _height);
    }
}
