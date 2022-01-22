using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// クリア画面UI操作クラス
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(ActionMode))]
[RequireComponent(typeof(Image))]
public class ClearUIController : MasterUIController
{
    /// <summary>アクションモード</summary>
    [SerializeField] private ActionMode act;

    protected override void OnEnable()
    {
        if (!act)
            act = GetComponent<ActionMode>();
        base.OnEnable();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (!act)
            act = GetComponent<ActionMode>();

        // 各項目によって変わる
        switch (act.clearMode)
        {
            case ClearActionMode.RetryAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)ClearActionMode.ProceedAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)ClearActionMode.SelectAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            case ClearActionMode.SelectAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)ClearActionMode.RetryAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)ClearActionMode.ProceedAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            case ClearActionMode.ProceedAction:
                if (!button)
                {
                    button = GetComponent<Button>();
                    var n = new Navigation();
                    n.mode = Navigation.Mode.Explicit;
                    n.selectOnUp = transform.parent.GetChild((int)ClearActionMode.SelectAction).GetComponent<Button>();
                    n.selectOnDown = transform.parent.GetChild((int)ClearActionMode.RetryAction).GetComponent<Button>();
                    button.navigation = n;
                }

                break;
            default:
                Debug.Log("アクションモード未設定");
                Debug.Log("オブジェクト名:[" + name + "]");
                break;
        }
    }

    /// <summary>
    /// イベントトリガーを設定する
    /// </summary>
    protected override void EntryEventTrigger()
    {
        base.EntryEventTrigger();
        var entryAry = new List<EventTrigger.Entry>();
        entryAry.Add(GetEntryEvent(EventTriggerType.Select, (data) => Selected()));
        entryAry.Add(GetEntryEvent(EventTriggerType.Deselect, (data) => Deselected()));
        entryAry.Add(GetEntryEvent(EventTriggerType.Submit, (data) => Submited()));
        eventTrigger.triggers = entryAry;
    }

    /// <summary>
    /// 選択項目の決定時に呼び出すメソッド
    /// </summary>
    public override void Submited()
    {
        // 各項目によって変わる
        switch (act.clearMode)
        {
            case ClearActionMode.RetryAction:
                if (_menuClose == false)
                {
                    _menuClose = true;
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                    SceneInfoManager.Instance.LoadSceneNameRedo();
                    LoadNow.Instance.gameObject.SetActive(true);
                    LoadNow.Instance.DrawLoadNowFadeOutTrigger = true;

                    button.enabled = false;
                    PlayFlashingMotion();
                }
                break;
            case ClearActionMode.SelectAction:
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
            case ClearActionMode.ProceedAction:
                if (_menuClose == false)
                {
                    _menuClose = true;
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                    SceneInfoManager.Instance.LoadSceneNameNext();
                    LoadNow.Instance.gameObject.SetActive(true);
                    LoadNow.Instance.DrawLoadNowFadeOutTrigger = true;

                    button.enabled = false;
                    PlayFlashingMotion();
                }
                break;
            default:
                break;
        }
    }
}
