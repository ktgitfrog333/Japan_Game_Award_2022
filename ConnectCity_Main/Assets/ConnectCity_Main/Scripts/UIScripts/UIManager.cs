using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ポーズ画面などのUIを制御する
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }

    private void Awake()
    {
        if (null != instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        // ポーズ画面表示の入力（クリア画面の表示中はポーズ画面を有効にしない）
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown(InputConst.INPUT_CONST_MENU) &&
                !ClearScreen.Instance.gameObject.activeSelf &&
                !PauseScreen.Instance.gameObject.activeSelf)
            .Subscribe(_ =>
            {
                PauseScreen.Instance.gameObject.SetActive(true);
                SfxPlay.Instance.PlaySFX(ClipToPlay.se_menu);
            });
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public async void CloseMenu()
    {
        await Task.Delay(500);
        // T.B.D プレイヤー/ギミックその他のオブジェクトを無効にする
        PauseScreen.Instance.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 遊び方確認を閉じる
    /// </summary>
    public async void CloseManual()
    {
        await Task.Delay(500);
        GameManualScrollView.Instance.ResetPage();
        GameManualScrollView.Instance.gameObject.SetActive(false);
        PauseScreen.Instance.AutoSelectContent(PauseActionMode.CheckAction);
    }

    /// <summary>
    /// クリア画面を開く
    /// </summary>
    public async void OpenClearScreen()
    {
        ClearScreen.Instance.gameObject.SetActive(true);

        // 子オブジェクトは一度非表示にする
        for (int i = 1; i < ClearScreen.Instance.transform.GetChild(0).childCount; i++)
            ClearScreen.Instance.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        await Task.Delay(3000);
        // 子オブジェクトは一度非表示にする
        for (int i = 1; i < ClearScreen.Instance.transform.GetChild(0).childCount; i++)
            ClearScreen.Instance.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);

        ClearScreen.Instance.AutoSelectContent();
    }
}
