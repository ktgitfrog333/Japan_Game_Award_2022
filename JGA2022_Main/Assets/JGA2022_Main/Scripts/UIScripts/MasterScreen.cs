using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    protected virtual void OnEnable() { }

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
            if (@event)
            {
                @event.SetSelectedGameObject(firstObject);
                SelectFirstElement();
            }
        }
    }

    /// <summary>
    /// 最初の項目を選択する
    /// </summary>
    protected virtual void SelectFirstElement() { }

    /// <summary>
    /// 初期設定
    /// </summary>
    protected virtual void Initialize() { }
}
