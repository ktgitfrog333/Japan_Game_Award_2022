using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;

namespace Gimmick
{
    /// <summary>
    /// ぼろい天井・床
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class BreakBlook : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP))
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
