using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Const;
using System.Threading.Tasks;

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

    void Update()
    {
        // ポーズ画面表示の入力
        if (Input.GetButtonDown(InputConst.INPUT_CONST_MENU))
        {
            PauseScreen.Instance.gameObject.SetActive(true);
        }
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
}
