using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gimmick
{
    /// <summary>
    /// ぼろい天井・床
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BreakBlook : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("MoveCubeGroup"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
