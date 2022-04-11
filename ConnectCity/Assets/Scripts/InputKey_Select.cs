using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class InputKey_Select : MonoBehaviour
{
    public GameObject Select_Flame_Image_01;
    public GameObject Select_Flame_Image_02;
    public GameObject Select_Flame_Image_03;
    public GameObject Select_Flame_Image_04;
    public GameObject Select_Flame_Image_05;
    public GameObject Select_Flame_Image_06;
    public GameObject Select_Flame_Image_07;
    public GameObject Select_Flame_Image_08;
    public GameObject Select_Flame_Image_09;
    public GameObject Select_Flame_Image_10;
    public GameObject Select_Flame_Image_11;
    public GameObject Select_Flame_Image_12;
    public GameObject Select_Flame_Image_13;
    public GameObject Select_Flame_Image_14;
    public GameObject Select_Flame_Image_15;
    public GameObject Select_Flame_Image_16;
    public GameObject Select_Flame_Image_17;
    public GameObject Select_Flame_Image_18;
    public GameObject Select_Flame_Image_19;
    public GameObject Select_Flame_Image_20;
    public GameObject Select_Flame_Image_21;
    public GameObject Select_Flame_Image_22;
    public GameObject Select_Flame_Image_23;
    public GameObject Select_Flame_Image_24;
    public GameObject Select_Flame_Image_25;
    public GameObject Select_Flame_Image_26;
    public GameObject Select_Flame_Image_27;
    public GameObject Select_Flame_Image_28;
    public GameObject Select_Flame_Image_29;
    public GameObject Select_Flame_Image_30;
    public GameObject Select_Left_Arrow;
    public GameObject Select_Right_Arrow;
    public GameObject Select_Stage_Frame;

    public Vector3 select_stage_pos;

    public Text text;
    public enum Stage_Num
    {
        Zero,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Stage7,
        Stage8,
        Stage9,
        Stage10,
        Stage11,
        Stage12,
        Stage13,
        Stage14,
        Stage15,
        Stage16,
        Stage17,
        Stage18,
        Stage19,
        Stage20,
        Stage21,
        Stage22,
        Stage23,
        Stage24,
        Stage25,
        Stage26,
        Stage27,
        Stage28,
        Stage29,
        Stage30
    }

    public enum Stage_Scroll
    {
        Zero,
        Scroll1,
        Scroll2,
        Scroll3,
        Scroll4,
        Scroll5,
        Scroll6
    }

    Stage_Num stage;
    Stage_Scroll scroll;
    // Start is called before the first frame update
    void Start()
    {
        Select_Flame_Image_01 = GameObject.Find("Select_Flame_Image_01");
        Select_Flame_Image_02 = GameObject.Find("Select_Flame_Image_02");
        Select_Flame_Image_03 = GameObject.Find("Select_Flame_Image_03");
        Select_Flame_Image_04 = GameObject.Find("Select_Flame_Image_04");
        Select_Flame_Image_05 = GameObject.Find("Select_Flame_Image_05");
        Select_Flame_Image_06 = GameObject.Find("Select_Flame_Image_06");
        Select_Flame_Image_07 = GameObject.Find("Select_Flame_Image_07");
        Select_Flame_Image_08 = GameObject.Find("Select_Flame_Image_08");
        Select_Flame_Image_09 = GameObject.Find("Select_Flame_Image_09");
        Select_Flame_Image_10 = GameObject.Find("Select_Flame_Image_10");
        Select_Flame_Image_11 = GameObject.Find("Select_Flame_Image_11");
        Select_Flame_Image_12 = GameObject.Find("Select_Flame_Image_12");
        Select_Flame_Image_13 = GameObject.Find("Select_Flame_Image_13");
        Select_Flame_Image_14 = GameObject.Find("Select_Flame_Image_14");
        Select_Flame_Image_15 = GameObject.Find("Select_Flame_Image_15");
        Select_Flame_Image_16 = GameObject.Find("Select_Flame_Image_16");
        Select_Flame_Image_17 = GameObject.Find("Select_Flame_Image_17");
        Select_Flame_Image_18 = GameObject.Find("Select_Flame_Image_18");
        Select_Flame_Image_19 = GameObject.Find("Select_Flame_Image_19");
        Select_Flame_Image_20 = GameObject.Find("Select_Flame_Image_20");
        Select_Flame_Image_21 = GameObject.Find("Select_Flame_Image_21");
        Select_Flame_Image_22 = GameObject.Find("Select_Flame_Image_22");
        Select_Flame_Image_23 = GameObject.Find("Select_Flame_Image_23");
        Select_Flame_Image_24 = GameObject.Find("Select_Flame_Image_24");
        Select_Flame_Image_25 = GameObject.Find("Select_Flame_Image_25");
        Select_Flame_Image_26 = GameObject.Find("Select_Flame_Image_26");
        Select_Flame_Image_27 = GameObject.Find("Select_Flame_Image_27");
        Select_Flame_Image_28 = GameObject.Find("Select_Flame_Image_28");
        Select_Flame_Image_29 = GameObject.Find("Select_Flame_Image_29");
        Select_Flame_Image_30 = GameObject.Find("Select_Flame_Image_30");
        Select_Left_Arrow = GameObject.Find("Select_Left_Arrow_Image");
        Select_Right_Arrow = GameObject.Find("Select_Right_Arrow_Image");
        Select_Stage_Frame = GameObject.Find("Select_Stage_Frame_Image");
        scroll = Stage_Scroll.Scroll1;
        stage = Stage_Num.Stage1;
        select_stage_pos = Select_Flame_Image_01.GetComponent<RectTransform>().anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        switch(scroll)
        {
            case Stage_Scroll.Scroll1:
                Select_Flame_Image_01.SetActive(true);
                Select_Flame_Image_02.SetActive(true);
                Select_Flame_Image_03.SetActive(true);
                Select_Flame_Image_04.SetActive(true);
                Select_Flame_Image_05.SetActive(true);
                Select_Flame_Image_06.SetActive(false);
                Select_Flame_Image_07.SetActive(false);
                Select_Flame_Image_08.SetActive(false);
                Select_Flame_Image_09.SetActive(false);
                Select_Flame_Image_10.SetActive(false);
                Select_Flame_Image_11.SetActive(false);
                Select_Flame_Image_12.SetActive(false);
                Select_Flame_Image_13.SetActive(false);
                Select_Flame_Image_14.SetActive(false);
                Select_Flame_Image_15.SetActive(false);
                Select_Flame_Image_16.SetActive(false);
                Select_Flame_Image_17.SetActive(false);
                Select_Flame_Image_18.SetActive(false);
                Select_Flame_Image_19.SetActive(false);
                Select_Flame_Image_20.SetActive(false);
                Select_Flame_Image_21.SetActive(false);
                Select_Flame_Image_22.SetActive(false);
                Select_Flame_Image_23.SetActive(false);
                Select_Flame_Image_24.SetActive(false);
                Select_Flame_Image_25.SetActive(false);
                Select_Flame_Image_26.SetActive(false);
                Select_Flame_Image_27.SetActive(false);
                Select_Flame_Image_28.SetActive(false);
                Select_Flame_Image_29.SetActive(false);
                Select_Flame_Image_30.SetActive(false);
                Select_Left_Arrow.SetActive(false);
                Select_Right_Arrow.SetActive(true);
                switch(stage)
                {
                    case Stage_Num.Stage1:
                        select_stage_pos = Select_Flame_Image_01.GetComponent<RectTransform>().anchoredPosition;
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage2;
                        }
                        break;
                }
                break;
        }
    }
}
