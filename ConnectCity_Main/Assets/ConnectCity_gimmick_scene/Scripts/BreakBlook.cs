using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;
using Main.Audio;

namespace Gimmick
{
    /// <summary>
    /// ぼろい天井・床
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class BreakBlook : MonoBehaviour
    {
        /// <summary>SE設定</summary>
        [SerializeField] private ClipToPlay breakBlookSE = ClipToPlay.se_collapse_No1;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) && 0f < collision.relativeVelocity.magnitude)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<Renderer>().enabled = false;
                SfxPlay.Instance.PlaySFX(breakBlookSE);
            }
        }
    }
}
