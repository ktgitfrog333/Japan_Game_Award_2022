using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// メニュー画面／クリア画面の親
/// </summary>
public class MasterScreen : MonoBehaviour
{
    /// <summary>選択項目のUIオブジェクト</summary>
    [SerializeField] protected GameObject firstObject;
    /// <summary>イベントシステム</summary>
    [SerializeField] protected EventSystem @event;

    protected virtual void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }
    private void Reset()
    {
        Initialize();
    }

    protected virtual void OnEnable() { }

    private void Start()
    {
        // マウスボタンが押されたら最初の項目を固定で選択する
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2))
            .Select(_ => @event)
            .Subscribe(_ =>
            {
                @event.SetSelectedGameObject(firstObject);
                SelectFirstElement();
            });
    }

    /// <summary>
    /// ある条件でデフォルト選択させる
    /// </summary>
    protected virtual void AutoSelectDefaultContent() { }

    /// <summary>
    /// 最初の項目を選択する
    /// </summary>
    protected virtual void SelectFirstElement() { }

    /// <summary>
    /// 初期設定
    /// </summary>
    protected virtual void Initialize() { }
}
