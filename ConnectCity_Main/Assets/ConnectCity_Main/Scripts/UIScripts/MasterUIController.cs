using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ポーズ項目／クリア項目の親
/// </summary>
public class MasterUIController : MonoBehaviour
{
    /// <summary>選択状態フレーム</summary>
    [SerializeField] protected GameObject frame;
    /// <summary>UIの共通制御</summary>
    protected UIControllerCommon _common;
    /// <summary>メニューを閉じる際に一度のみ実行するよう制御するフラグ</summary>
    protected bool _menuClose;
    /// <summary>ボタン</summary>
    [SerializeField] protected Button button;
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

    protected virtual void OnEnable()
    {
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
    protected virtual void Initialize()
    {
        // 共通処理
        if (!frame)
        {
            frame = transform.GetChild(0).gameObject;
            frame.SetActive(false);
        }
        // ボタン画像のコンポーネントを取得
        if (!image)
            image = GetComponent<Image>();
    }

    /// <summary>
    /// イベントトリガーを設定する
    /// </summary>
    protected virtual void EntryEventTrigger() { }

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
    public virtual void Submited() { }

    /// <summary>
    /// 点滅演出
    /// </summary>
    protected async void PlayFlashingMotion()
    {
        // 点滅の演出
        await Task.Delay(80);
        image.DOColor(Color.gray, .3f);
        await Task.Delay(80);
        image.DOColor(Color.white, .3f);
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
