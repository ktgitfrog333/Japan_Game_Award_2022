using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common
{
    /// <summary>
    /// マウス操作を禁止
    /// </summary>
    public class MouseCursorBan : MonoBehaviour
    {
        private void Start()
        {
            Cursor.visible = false;
        }
    }
}
