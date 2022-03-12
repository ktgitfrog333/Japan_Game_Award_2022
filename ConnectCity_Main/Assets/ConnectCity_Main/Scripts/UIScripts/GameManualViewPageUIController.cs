using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 操作説明UI操作クラス
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
public class GameManualViewPageUIController : MonoBehaviour
{
    /// <summary>ページ番号</summary>
    private int _pageIndex;
    /// <summary>ボタン</summary>
    [SerializeField] private Button button;
    /// <summary>イベントシステム</summary>
    [SerializeField] private EventTrigger eventTrigger;
    /// <summary>メニューを閉じる際に一度のみ実行するよう制御するフラグ</summary>
    private bool _menuClose;

    private void Awake()
    {
        EntryEventTrigger();
    }

    private void OnEnable()
    {
        _menuClose = false;
        if (!button.isActiveAndEnabled)
            button.enabled = true;
    }

    private void Reset()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // 自身のページ番号を取得
        for (var i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).name.Equals(name))
            {
                _pageIndex = i;
                break;
            }
        }
        // ページ遷移の設定
        for (var i = 0; i < transform.parent.childCount; i++)
        {
            if (i == _pageIndex)
            {
                if (!button)
                    button = GetComponent<Button>();
                var n = new Navigation();
                n.mode = Navigation.Mode.Explicit;
                // 一つ前のページをナビゲーションへセット
                if (-1 < (i - 1))
                    n.selectOnLeft = transform.parent.GetChild(i - 1).GetComponent<Button>();
                // 次のページをナビゲーションへセット
                if ((i + 1) < transform.parent.childCount)
                    n.selectOnRight = transform.parent.GetChild(i + 1).GetComponent<Button>();
                button.navigation = n;
                break;
            }
        }
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
        SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
        GameManualScrollView.Instance.ScrollPage(_pageIndex);
    }

    /// <summary>
    /// 選択項目の決定時に呼び出すメソッド
    /// </summary>
    public void Submited()
    {
        if (!_menuClose)
        {
            _menuClose = true;
            SfxPlay.Instance.PlaySFX(ClipToPlay.se_close);
            UIManager.Instance.CloseManual();
            button.enabled = false;
        }
    }

    public void Canceled()
    {
        if (!_menuClose)
        {
            _menuClose = true;
            SfxPlay.Instance.PlaySFX(ClipToPlay.se_close);
            UIManager.Instance.CloseManual();
            button.enabled = false;
        }
    }
}
