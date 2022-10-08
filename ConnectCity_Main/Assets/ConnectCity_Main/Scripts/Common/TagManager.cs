using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Main.Common.Const;

namespace Main.Common
{
    /// <summary>
    /// 下記からタグマネージャーの取得、修正
    /// ProjectSettings/TagManager.asset
    /// https://qiita.com/UnagiHuman/items/ba7ef0932e5ebec0813f
    /// </summary>
    public class TagManager : MonoBehaviour
    {
        //設定したいレイヤーのレイヤー名
        private string[] requiredLayers =
        {
            LayerConst.LAYER_NAME_PLAYER,
            LayerConst.LAYER_NAME_FREEZE,
            LayerConst.LAYER_NAME_MOVECUBE,
            LayerConst.LAYER_NAME_ROBOTENEMIES,
            LayerConst.LAYER_NAME_WARPGATE,
        };

        //設定したいレイヤーのレイヤー番号
        private int[] requiredLayerIds =
        {
            6,
            7,
            8,
            9,
            10,
        };

        private void Reset()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            CheckLayers(tagManager);
            tagManager.ApplyModifiedProperties();
        }

        /// <summary>
        /// 設定したいレイヤーを上書きする
        /// </summary>
        /// <param name="tagManager"></param>
        private void CheckLayers(SerializedObject tagManager)
        {
            //layer情報を取得
            var layersProp = tagManager.FindProperty("layers");
            var index = 0;
            foreach (var layerId in requiredLayerIds)
            {
                if (layersProp.arraySize > layerId)
                {
                    //レイヤーIDのレイヤーを取得
                    var sp = layersProp.GetArrayElementAtIndex(layerId);
                    if (sp != null && sp.stringValue != requiredLayers[index])
                    {
                        //レイヤー上書き
                        sp.stringValue = requiredLayers[index];
                        Debug.Log("Adding layer " + requiredLayers[index]);
                    }
                }

                index++;
            }
        }
    }
}
