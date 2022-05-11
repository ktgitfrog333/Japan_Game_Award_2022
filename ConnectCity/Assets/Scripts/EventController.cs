using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace TitleSelect
{
    /// <summary>
    /// イベントコントローラー
    /// </summary>
    public class EventController : MonoBehaviour
    {
        /// <summary>
        /// 実行イベントの監視
        /// 0 : Selected
        /// 1 : DeSelected
        /// 2 : Submited
        /// 3 : Canceled
        /// </summary>
        public IntReactiveProperty EventRP { get; set; } = new IntReactiveProperty(-1);

        /// <summary>
        /// 選択された時に発火するイベント
        /// </summary>
        public void Selected()
        {
            EventRP.Value = 0;
        }

        /// <summary>
        /// 選択されなかった時に発火するイベント
        /// </summary>
        public void DeSelected()
        {
            EventRP.Value = 1;
        }

        /// <summary>
        /// 確定時に発火するイベント
        /// </summary>
        public void Submited()
        {
            EventRP.Value = 2;
        }

        /// <summary>
        /// キャンセル時に発火するイベント
        /// </summary>
        public void Canceled()
        {
            EventRP.Value = 3;
        }
    }
}
