using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Level
{
    /// <summary>
    /// Skyboxの設定管理
    /// </summary>
    public class SkyBoxSet : MonoBehaviour
    {
        /// <summary>ステージごとのSkybox</summary>
        [SerializeField] private Material[] skyboxs;

        /// <summary>
        /// スカイボックスを設定
        /// </summary>
        /// <param name="idx">パターン番号</param>
        /// <returns></returns>
        public bool SetRenderSkybox(RenderSettingsSkybox idx)
        {
            try
            {
                // Skyboxの設定
                RenderSettings.skybox = skyboxs[(int)idx];
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Skyboxのマテリアルリソース
    /// </summary>
    public enum RenderSettingsSkybox
    {
        mat_cubemap_desertvillage,
        mat_cubemap_ghostcity,
        mat_cubemap_skyscrapers,
    }
}
