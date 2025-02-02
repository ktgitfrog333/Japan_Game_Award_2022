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
    public Vector3 select_flame_image_01_pos;
    public Vector3 select_flame_image_02_pos;
    public Vector3 select_flame_image_03_pos;
    public Vector3 select_flame_image_04_pos;
    public Vector3 select_flame_image_05_pos;
    public Vector3 select_flame_image_06_pos;
    public Vector3 select_flame_image_07_pos;
    public Vector3 select_flame_image_08_pos;
    public Vector3 select_flame_image_09_pos;
    public Vector3 select_flame_image_10_pos;
    public Vector3 select_flame_image_11_pos;
    public Vector3 select_flame_image_12_pos;
    public Vector3 select_flame_image_13_pos;
    public Vector3 select_flame_image_14_pos;
    public Vector3 select_flame_image_15_pos;
    public Vector3 select_flame_image_16_pos;
    public Vector3 select_flame_image_17_pos;
    public Vector3 select_flame_image_18_pos;
    public Vector3 select_flame_image_19_pos;
    public Vector3 select_flame_image_20_pos;
    public Vector3 select_flame_image_21_pos;
    public Vector3 select_flame_image_22_pos;
    public Vector3 select_flame_image_23_pos;
    public Vector3 select_flame_image_24_pos;
    public Vector3 select_flame_image_25_pos;
    public Vector3 select_flame_image_26_pos;
    public Vector3 select_flame_image_27_pos;
    public Vector3 select_flame_image_28_pos;
    public Vector3 select_flame_image_29_pos;
    public Vector3 select_flame_image_30_pos;

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
        select_stage_pos = Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_01_pos = Select_Flame_Image_01.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_02_pos = Select_Flame_Image_02.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_03_pos = Select_Flame_Image_03.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_04_pos = Select_Flame_Image_04.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_05_pos = Select_Flame_Image_05.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_06_pos = Select_Flame_Image_06.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_07_pos = Select_Flame_Image_07.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_08_pos = Select_Flame_Image_08.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_09_pos = Select_Flame_Image_09.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_10_pos = Select_Flame_Image_10.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_11_pos = Select_Flame_Image_11.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_12_pos = Select_Flame_Image_12.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_13_pos = Select_Flame_Image_13.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_14_pos = Select_Flame_Image_14.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_15_pos = Select_Flame_Image_15.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_16_pos = Select_Flame_Image_16.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_17_pos = Select_Flame_Image_17.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_18_pos = Select_Flame_Image_18.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_19_pos = Select_Flame_Image_19.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_20_pos = Select_Flame_Image_20.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_21_pos = Select_Flame_Image_21.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_22_pos = Select_Flame_Image_22.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_23_pos = Select_Flame_Image_23.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_24_pos = Select_Flame_Image_24.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_25_pos = Select_Flame_Image_25.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_26_pos = Select_Flame_Image_26.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_27_pos = Select_Flame_Image_27.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_28_pos = Select_Flame_Image_28.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_29_pos = Select_Flame_Image_29.GetComponent<RectTransform>().anchoredPosition;
        select_flame_image_30_pos = Select_Flame_Image_30.GetComponent<RectTransform>().anchoredPosition;
    }

    // Update is called once per frame
    async public void Update()
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
                        select_stage_pos.x = select_flame_image_01_pos.x;
                        select_stage_pos.y = select_flame_image_01_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage1);
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage2;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage2:
                        select_stage_pos.x = select_flame_image_02_pos.x;
                        select_stage_pos.y = select_flame_image_02_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage2);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage1;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage3;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage3:
                        select_stage_pos.x = select_flame_image_03_pos.x;
                        select_stage_pos.y = select_flame_image_03_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage3);
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage2;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage4;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage4:
                        select_stage_pos.x = select_flame_image_04_pos.x;
                        select_stage_pos.y = select_flame_image_04_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage4);
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage3;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage5;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage5:
                        select_stage_pos.x = select_flame_image_05_pos.x;
                        select_stage_pos.y = select_flame_image_05_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage5);
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage4;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            scroll = Stage_Scroll.Scroll2;
                            stage = Stage_Num.Stage6;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;
                }
                break;

            case Stage_Scroll.Scroll2:
                Select_Flame_Image_01.SetActive(false);
                Select_Flame_Image_02.SetActive(false);
                Select_Flame_Image_03.SetActive(false);
                Select_Flame_Image_04.SetActive(false);
                Select_Flame_Image_05.SetActive(false);
                Select_Flame_Image_06.SetActive(true);
                Select_Flame_Image_07.SetActive(true);
                Select_Flame_Image_08.SetActive(true);
                Select_Flame_Image_09.SetActive(true);
                Select_Flame_Image_10.SetActive(true);
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
                Select_Left_Arrow.SetActive(true);
                Select_Right_Arrow.SetActive(true);
                switch(stage)
                {
                    case Stage_Num.Stage6:
                        select_stage_pos.x = select_flame_image_06_pos.x;
                        select_stage_pos.y = select_flame_image_06_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage6);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            scroll = Stage_Scroll.Scroll1;
                            stage = Stage_Num.Stage5;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage7;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage7:
                        select_stage_pos.x = select_flame_image_07_pos.x;
                        select_stage_pos.y = select_flame_image_07_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage7);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage6;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage8;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage8:
                        select_stage_pos.x = select_flame_image_08_pos.x;
                        select_stage_pos.y = select_flame_image_08_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage8);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage7;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage9;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage9:
                        select_stage_pos.x = select_flame_image_09_pos.x;
                        select_stage_pos.y = select_flame_image_09_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage9);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage8;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage10;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage10:
                        select_stage_pos.x = select_flame_image_10_pos.x;
                        select_stage_pos.y = select_flame_image_10_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage10);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage9;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            scroll = Stage_Scroll.Scroll3;
                            stage = Stage_Num.Stage11;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;
                }
                break;

            case Stage_Scroll.Scroll3:
                Select_Flame_Image_01.SetActive(false);
                Select_Flame_Image_02.SetActive(false);
                Select_Flame_Image_03.SetActive(false);
                Select_Flame_Image_04.SetActive(false);
                Select_Flame_Image_05.SetActive(false);
                Select_Flame_Image_06.SetActive(false);
                Select_Flame_Image_07.SetActive(false);
                Select_Flame_Image_08.SetActive(false);
                Select_Flame_Image_09.SetActive(false);
                Select_Flame_Image_10.SetActive(false);
                Select_Flame_Image_11.SetActive(true);
                Select_Flame_Image_12.SetActive(true);
                Select_Flame_Image_13.SetActive(true);
                Select_Flame_Image_14.SetActive(true);
                Select_Flame_Image_15.SetActive(true);
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
                Select_Left_Arrow.SetActive(true);
                Select_Right_Arrow.SetActive(true);
                switch(stage)
                {
                    case Stage_Num.Stage11:
                        select_stage_pos.x = select_flame_image_11_pos.x;
                        select_stage_pos.y = select_flame_image_11_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage11);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            scroll = Stage_Scroll.Scroll2;
                            stage = Stage_Num.Stage10;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage12;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage12:
                        select_stage_pos.x = select_flame_image_12_pos.x;
                        select_stage_pos.y = select_flame_image_12_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage12);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage11;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage13;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage13:
                        select_stage_pos.x = select_flame_image_13_pos.x;
                        select_stage_pos.y = select_flame_image_13_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage13);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage12;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage14;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage14:
                        select_stage_pos.x = select_flame_image_14_pos.x;
                        select_stage_pos.y = select_flame_image_14_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage14);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage13;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage15;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage15:
                        select_stage_pos.x = select_flame_image_15_pos.x;
                        select_stage_pos.y = select_flame_image_15_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage15);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage14;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            scroll = Stage_Scroll.Scroll4;
                            stage = Stage_Num.Stage16;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;
                }
                break;

            case Stage_Scroll.Scroll4:
                Select_Flame_Image_01.SetActive(false);
                Select_Flame_Image_02.SetActive(false);
                Select_Flame_Image_03.SetActive(false);
                Select_Flame_Image_04.SetActive(false);
                Select_Flame_Image_05.SetActive(false);
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
                Select_Flame_Image_16.SetActive(true);
                Select_Flame_Image_17.SetActive(true);
                Select_Flame_Image_18.SetActive(true);
                Select_Flame_Image_19.SetActive(true);
                Select_Flame_Image_20.SetActive(true);
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
                Select_Left_Arrow.SetActive(true);
                Select_Right_Arrow.SetActive(true);
                switch(stage)
                {
                    case Stage_Num.Stage16:
                        select_stage_pos.x = select_flame_image_16_pos.x;
                        select_stage_pos.y = select_flame_image_16_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage16);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            scroll = Stage_Scroll.Scroll3;
                            stage = Stage_Num.Stage15;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage17;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage17:
                        select_stage_pos.x = select_flame_image_17_pos.x;
                        select_stage_pos.y = select_flame_image_17_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage17);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage16;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage18;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage18:
                        select_stage_pos.x = select_flame_image_18_pos.x;
                        select_stage_pos.y = select_flame_image_18_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage18);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage17;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage19;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage19:
                        select_stage_pos.x = select_flame_image_19_pos.x;
                        select_stage_pos.y = select_flame_image_19_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage19);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage18;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage20;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage20:
                        select_stage_pos.x = select_flame_image_20_pos.x;
                        select_stage_pos.y = select_flame_image_20_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage20);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage19;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            scroll = Stage_Scroll.Scroll5;
                            stage = Stage_Num.Stage21;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;
                }
                break;

            case Stage_Scroll.Scroll5:
                Select_Flame_Image_01.SetActive(false);
                Select_Flame_Image_02.SetActive(false);
                Select_Flame_Image_03.SetActive(false);
                Select_Flame_Image_04.SetActive(false);
                Select_Flame_Image_05.SetActive(false);
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
                Select_Flame_Image_21.SetActive(true);
                Select_Flame_Image_22.SetActive(true);
                Select_Flame_Image_23.SetActive(true);
                Select_Flame_Image_24.SetActive(true);
                Select_Flame_Image_25.SetActive(true);
                Select_Flame_Image_26.SetActive(false);
                Select_Flame_Image_27.SetActive(false);
                Select_Flame_Image_28.SetActive(false);
                Select_Flame_Image_29.SetActive(false);
                Select_Flame_Image_30.SetActive(false);
                Select_Left_Arrow.SetActive(true);
                Select_Right_Arrow.SetActive(true);
                switch(stage)
                {
                    case Stage_Num.Stage21:
                        select_stage_pos.x = select_flame_image_21_pos.x;
                        select_stage_pos.y = select_flame_image_21_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage21);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            scroll = Stage_Scroll.Scroll4;
                            stage = Stage_Num.Stage20;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage22;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage22:
                        select_stage_pos.x = select_flame_image_22_pos.x;
                        select_stage_pos.y = select_flame_image_22_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage22);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage21;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage23;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage23:
                        select_stage_pos.x = select_flame_image_23_pos.x;
                        select_stage_pos.y = select_flame_image_23_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage23);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage22;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage24;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage24:
                        select_stage_pos.x = select_flame_image_24_pos.x;
                        select_stage_pos.y = select_flame_image_24_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage24);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage23;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage25;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage25:
                        select_stage_pos.x = select_flame_image_25_pos.x;
                        select_stage_pos.y = select_flame_image_25_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage25);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage24;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            scroll = Stage_Scroll.Scroll6;
                            stage = Stage_Num.Stage26;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;
                }
                break;

            case Stage_Scroll.Scroll6:
                Select_Flame_Image_01.SetActive(false);
                Select_Flame_Image_02.SetActive(false);
                Select_Flame_Image_03.SetActive(false);
                Select_Flame_Image_04.SetActive(false);
                Select_Flame_Image_05.SetActive(false);
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
                Select_Flame_Image_26.SetActive(true);
                Select_Flame_Image_27.SetActive(true);
                Select_Flame_Image_28.SetActive(true);
                Select_Flame_Image_29.SetActive(true);
                Select_Flame_Image_30.SetActive(true);
                Select_Left_Arrow.SetActive(false);
                Select_Right_Arrow.SetActive(true);
                switch(stage)
                {
                    case Stage_Num.Stage26:
                        select_stage_pos.x = select_flame_image_26_pos.x;
                        select_stage_pos.y = select_flame_image_26_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage26);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            scroll = Stage_Scroll.Scroll5;
                            stage = Stage_Num.Stage25;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage27;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage27:
                        select_stage_pos.x = select_flame_image_27_pos.x;
                        select_stage_pos.y = select_flame_image_27_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage27);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage26;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage28;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage28:
                        select_stage_pos.x = select_flame_image_28_pos.x;
                        select_stage_pos.y = select_flame_image_28_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage28);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage27;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage29;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage29:
                        select_stage_pos.x = select_flame_image_29_pos.x;
                        select_stage_pos.y = select_flame_image_29_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage29);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage28;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            stage = Stage_Num.Stage30;
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;

                    case Stage_Num.Stage30:
                        select_stage_pos.x = select_flame_image_30_pos.x;
                        select_stage_pos.y = select_flame_image_30_pos.y;
                        Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                        GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage30);
                        if(Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            stage = Stage_Num.Stage29;
                        }
                        if(Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            //
                        }
                        if(Input.GetKeyDown(KeyCode.Space))
                        {
                            //
                        }
                        break;
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("FadeInOutPanel").GetComponent<FadeInOut>().Fadeout();
            await Task.Delay(3000);
            SceneManager.LoadScene("TitleScene");
        }
    }
}
