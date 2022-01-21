using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Common.Const;

/// <summary>
/// メニュー画面制御クラス
/// </summary>
public class PauseScreen : MonoBehaviour
{
    private static PauseScreen instance;
    public static PauseScreen Instance { get { return instance; } }
    /// <summary>選択項目のUIスクリプト</summary>
    [SerializeField] private UIController firstElement;
    /// <summary>選択項目のUIオブジェクト</summary>
    [SerializeField] private GameObject firstObject;
    /// <summary>イベントシステム</summary>
    [SerializeField] private EventSystem @event;

    private void Awake()
    {
        if (null != instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Initialize();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        @event.SetSelectedGameObject(firstObject);
        firstElement.Selected();
        SfxPlay.Instance.PlaySFX(ClipToPlay.se_menu);
    }

    private void Reset()
    {
        Initialize();
    }

    private void Update()
    {
        // マウスボタンが押されたら最初の項目を固定で選択する
        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))
        {
            @event.SetSelectedGameObject(firstObject);
            firstElement.Selected();
        }
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    private void Initialize()
    {
        if (!firstElement)
            firstElement = transform.GetChild(0).GetChild(1).GetComponent<UIController>();
        if (!firstObject)
            firstObject = GameObject.Find("GameBackButton");
        if (!@event)
            @event = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    /// <summary>
    /// システムから項目を自動で選択させたい場合に使用する
    /// </summary>
    /// <param name="mode">ポーズ画面の項目</param>
    public void AutoSelectContent(PauseActionMode mode)
    {
        var g = transform.GetChild(0).GetChild((int)mode).gameObject;
        @event.SetSelectedGameObject(g);
        g.GetComponent<UIController>().Selected();
    }
}
