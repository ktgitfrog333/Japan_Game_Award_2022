using Main.Common;
using Main.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;

public class Test : MonoBehaviour
{
    [SerializeField, Range(0, 29)] private int updateScenesMap = 0;
    private bool flag;
    private bool singleOpe;

    private void Start()
    {
        //Debug.Log(BrideScenes_SelectMain.Instance.LoadSceneId);
        GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().UpdateScenesMap(updateScenesMap);
    }

    private void Update()
    {
        if (flag)
        {
            if (!singleOpe)
            {
                singleOpe = true;
                BrideScenes_SelectMain.Instance.SetMainSceneNameIdFromSelect_Scene(0);
                BrideScenes_SelectMain.Instance.PlayLoadScene();
            }
        }
        //Debug.Log(Input.GetAxis(InputConst.INPUT_CONST_HORIZONTAL_RS_KEYBOD));
    }

    //[SerializeField] private bool gravityControllerActive;
    //[SerializeField] private Direction direction;
    //[SerializeField, Range(0.1f, 8f)] private float gravity = .1f;

}
