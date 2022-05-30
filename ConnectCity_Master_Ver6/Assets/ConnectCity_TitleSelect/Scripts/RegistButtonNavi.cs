using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TitleSelect
{
    /// <summary>
    /// ボタンコンポーネントの追加とナビゲーションをセットする
    /// リセット後は消してもOK
    /// </summary>
    public class RegistButtonNavi : MonoBehaviour
    {
        private void Reset()
        {
            // 子要素を取得してボタンコンポーネントを参照
            foreach (Transform t in transform)
            {
                var btn = t.GetComponent<Button>();
                if (btn == null)
                    t.gameObject.AddComponent<Button>();
            }

            var chidMax = transform.childCount;
            for (var i = 0; i < chidMax; i++)
            {
                // 子要素を取得してボタンコンポーネントを参照
                var btn = transform.GetChild(i).GetComponent<Button>();

                // 最初と最後は処理を行わない
                var nav = new Navigation();
                nav.mode = Navigation.Mode.Explicit;
                if (0 < i)
                    nav.selectOnUp = transform.GetChild(i - 1).GetComponent<Button>();
                if (i < chidMax - 1)
                    nav.selectOnDown = transform.GetChild(i + 1).GetComponent<Button>();

                btn.navigation = nav;
            }
        }
    }
}
