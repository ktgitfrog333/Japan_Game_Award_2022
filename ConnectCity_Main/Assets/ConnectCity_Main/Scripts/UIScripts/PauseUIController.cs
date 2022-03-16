using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ポーズ画面UI操作クラス
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(ActionMode))]
[RequireComponent(typeof(Image))]
public class PauseUIController : MasterUIController
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
        switch (act.pauseMode)
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
    }

    /// <summary>
    /// イベントトリガーを設定する
    /// </summary>
    protected override void EntryEventTrigger()
    {
        button.OnSelectAsObservable()
            .Subscribe(_ => Selected());
        button.OnDeselectAsObservable()
            .Subscribe(_ => Deselected());
        button.OnSubmitAsObservable()
            .Subscribe(_ => Submited());
        button.OnCancelAsObservable()
            .Subscribe(_ => Canceled());
    }

    /// <summary>
    /// 選択項目の決定時に呼び出すメソッド
    /// </summary>
    public override void Submited()
    {
        // 各項目によって変わる
        switch (act.pauseMode)
        {
            case PauseActionMode.BackAction:
                if (_menuClose == false)
                {
                    _menuClose = true;
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_close);
                    UIManager.Instance.CloseMenu();
                    button.enabled = false;
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
        }
    }
}
