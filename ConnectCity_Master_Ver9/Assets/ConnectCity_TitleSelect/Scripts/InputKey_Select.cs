using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using Main.Common;
using UniRx;
using UnityEngine.InputSystem;

namespace TitleSelect
{
    public class InputKey_Select : MonoBehaviour, IInputCallbackValue
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
        public GameObject Select_Flame_Image_31;
        public GameObject Select_Flame_Image_32;
        public GameObject Select_Flame_Image_33;
        public GameObject Select_Flame_Image_34;
        public GameObject Select_Flame_Image_35;
        public GameObject Select_Flame_Image_36;
        public GameObject Select_Flame_Image_37;
        public GameObject Select_Flame_Image_38;
        public GameObject Select_Flame_Image_39;
        public GameObject Select_Flame_Image_40;
        public GameObject Select_Flame_Image_41;
        public GameObject Select_Flame_Image_42;
        public GameObject Select_Flame_Image_43;
        public GameObject Select_Flame_Image_44;
        public GameObject Select_Flame_Image_45;
        public GameObject Select_Flame_Image_46;
        public GameObject Select_Flame_Image_47;
        public GameObject Select_Flame_Image_48;
        public GameObject Select_Flame_Image_49;
        public GameObject Select_Flame_Image_50;

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
        public Vector3 select_flame_image_31_pos;
        public Vector3 select_flame_image_32_pos;
        public Vector3 select_flame_image_33_pos;
        public Vector3 select_flame_image_34_pos;
        public Vector3 select_flame_image_35_pos;
        public Vector3 select_flame_image_36_pos;
        public Vector3 select_flame_image_37_pos;
        public Vector3 select_flame_image_38_pos;
        public Vector3 select_flame_image_39_pos;
        public Vector3 select_flame_image_40_pos;
        public Vector3 select_flame_image_41_pos;
        public Vector3 select_flame_image_42_pos;
        public Vector3 select_flame_image_43_pos;
        public Vector3 select_flame_image_44_pos;
        public Vector3 select_flame_image_45_pos;
        public Vector3 select_flame_image_46_pos;
        public Vector3 select_flame_image_47_pos;
        public Vector3 select_flame_image_48_pos;
        public Vector3 select_flame_image_49_pos;
        public Vector3 select_flame_image_50_pos;

        bool select_decide;
        bool select_cancel;

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
            Stage30,
            Stage31,
            Stage32,
            Stage33,
            Stage34,
            Stage35,
            Stage36,
            Stage37,
            Stage38,
            Stage39,
            Stage40,
            Stage41,
            Stage42,
            Stage43,
            Stage44,
            Stage45,
            Stage46,
            Stage47,
            Stage48,
            Stage49,
            Stage50,
        }

        public enum Stage_Scroll
        {
            Zero,
            Scroll1,
            Scroll2,
            Scroll3,
            Scroll4,
            Scroll5,
            Scroll6,
            Scroll7,
            Scroll8,
            Scroll9,
            Scroll10,
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
            Select_Flame_Image_31 = GameObject.Find("Select_Flame_Image_31");
            Select_Flame_Image_32 = GameObject.Find("Select_Flame_Image_32");
            Select_Flame_Image_33 = GameObject.Find("Select_Flame_Image_33");
            Select_Flame_Image_34 = GameObject.Find("Select_Flame_Image_34");
            Select_Flame_Image_35 = GameObject.Find("Select_Flame_Image_35");
            Select_Flame_Image_36 = GameObject.Find("Select_Flame_Image_36");
            Select_Flame_Image_37 = GameObject.Find("Select_Flame_Image_37");
            Select_Flame_Image_38 = GameObject.Find("Select_Flame_Image_38");
            Select_Flame_Image_39 = GameObject.Find("Select_Flame_Image_39");
            Select_Flame_Image_40 = GameObject.Find("Select_Flame_Image_40");
            Select_Flame_Image_41 = GameObject.Find("Select_Flame_Image_41");
            Select_Flame_Image_42 = GameObject.Find("Select_Flame_Image_42");
            Select_Flame_Image_43 = GameObject.Find("Select_Flame_Image_43");
            Select_Flame_Image_44 = GameObject.Find("Select_Flame_Image_44");
            Select_Flame_Image_45 = GameObject.Find("Select_Flame_Image_45");
            Select_Flame_Image_46 = GameObject.Find("Select_Flame_Image_46");
            Select_Flame_Image_47 = GameObject.Find("Select_Flame_Image_47");
            Select_Flame_Image_48 = GameObject.Find("Select_Flame_Image_48");
            Select_Flame_Image_49 = GameObject.Find("Select_Flame_Image_49");
            Select_Flame_Image_50 = GameObject.Find("Select_Flame_Image_50");
            Select_Left_Arrow = GameObject.Find("Select_Left_Arrow_Image");
            Select_Right_Arrow = GameObject.Find("Select_Right_Arrow_Image");
            Select_Stage_Frame = GameObject.Find("Select_Stage_Frame_Image");
            select_decide = false;
            select_cancel = false;
            //stage = Stage_Num.Stage1;

            stage = (Stage_Num)BrideScenes_SelectMain.Instance.LoadSceneId + 1;
            if ((int)Stage_Num.Stage1 <= (int)stage && (int)stage <= (int)Stage_Num.Stage5)
            {
                scroll = Stage_Scroll.Scroll1;
            }
            else if ((int)Stage_Num.Stage6 <= (int)stage && (int)stage <= (int)Stage_Num.Stage10)
            {
                scroll = Stage_Scroll.Scroll2;
            }
            else if ((int)Stage_Num.Stage11 <= (int)stage && (int)stage <= (int)Stage_Num.Stage15)
            {
                scroll = Stage_Scroll.Scroll3;
            }
            else if ((int)Stage_Num.Stage16 <= (int)stage && (int)stage <= (int)Stage_Num.Stage20)
            {
                scroll = Stage_Scroll.Scroll4;
            }
            else if ((int)Stage_Num.Stage21 <= (int)stage && (int)stage <= (int)Stage_Num.Stage25)
            {
                scroll = Stage_Scroll.Scroll5;
            }
            else if ((int)Stage_Num.Stage26 <= (int)stage && (int)stage <= (int)Stage_Num.Stage30)
            {
                scroll = Stage_Scroll.Scroll6;
            }
            else if ((int)Stage_Num.Stage31 <= (int)stage && (int)stage <= (int)Stage_Num.Stage35)
            {
                scroll = Stage_Scroll.Scroll7;
            }
            else if ((int)Stage_Num.Stage36 <= (int)stage && (int)stage <= (int)Stage_Num.Stage40)
            {
                scroll = Stage_Scroll.Scroll8;
            }
            else if ((int)Stage_Num.Stage41 <= (int)stage && (int)stage <= (int)Stage_Num.Stage45)
            {
                scroll = Stage_Scroll.Scroll9;
            }
            else if ((int)Stage_Num.Stage46 <= (int)stage && (int)stage <= (int)Stage_Num.Stage50)
            {
                scroll = Stage_Scroll.Scroll10;
            }
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
            select_flame_image_31_pos = Select_Flame_Image_31.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_32_pos = Select_Flame_Image_32.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_33_pos = Select_Flame_Image_33.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_34_pos = Select_Flame_Image_34.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_35_pos = Select_Flame_Image_35.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_36_pos = Select_Flame_Image_36.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_37_pos = Select_Flame_Image_37.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_38_pos = Select_Flame_Image_38.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_39_pos = Select_Flame_Image_39.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_40_pos = Select_Flame_Image_40.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_41_pos = Select_Flame_Image_41.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_42_pos = Select_Flame_Image_42.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_43_pos = Select_Flame_Image_43.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_44_pos = Select_Flame_Image_44.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_45_pos = Select_Flame_Image_45.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_46_pos = Select_Flame_Image_46.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_47_pos = Select_Flame_Image_47.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_48_pos = Select_Flame_Image_48.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_49_pos = Select_Flame_Image_49.GetComponent<RectTransform>().anchoredPosition;
            select_flame_image_50_pos = Select_Flame_Image_50.GetComponent<RectTransform>().anchoredPosition;

            Cursor.visible = false;
        }

        /// <summary>
        /// ステージのコンテンツ描画を切り替える
        /// </summary>
        /// <param name="scroll">スクロール番号</param>
        private void RenderUpdateSelectFlameImage(Stage_Scroll scroll)
        {
            try
            {
                Select_Flame_Image_01.SetActive(scroll.Equals(Stage_Scroll.Scroll1));
                Select_Flame_Image_02.SetActive(scroll.Equals(Stage_Scroll.Scroll1));
                Select_Flame_Image_03.SetActive(scroll.Equals(Stage_Scroll.Scroll1));
                Select_Flame_Image_04.SetActive(scroll.Equals(Stage_Scroll.Scroll1));
                Select_Flame_Image_05.SetActive(scroll.Equals(Stage_Scroll.Scroll1));
                Select_Flame_Image_06.SetActive(scroll.Equals(Stage_Scroll.Scroll2));
                Select_Flame_Image_07.SetActive(scroll.Equals(Stage_Scroll.Scroll2));
                Select_Flame_Image_08.SetActive(scroll.Equals(Stage_Scroll.Scroll2));
                Select_Flame_Image_09.SetActive(scroll.Equals(Stage_Scroll.Scroll2));
                Select_Flame_Image_10.SetActive(scroll.Equals(Stage_Scroll.Scroll2));
                Select_Flame_Image_11.SetActive(scroll.Equals(Stage_Scroll.Scroll3));
                Select_Flame_Image_12.SetActive(scroll.Equals(Stage_Scroll.Scroll3));
                Select_Flame_Image_13.SetActive(scroll.Equals(Stage_Scroll.Scroll3));
                Select_Flame_Image_14.SetActive(scroll.Equals(Stage_Scroll.Scroll3));
                Select_Flame_Image_15.SetActive(scroll.Equals(Stage_Scroll.Scroll3));
                Select_Flame_Image_16.SetActive(scroll.Equals(Stage_Scroll.Scroll4));
                Select_Flame_Image_17.SetActive(scroll.Equals(Stage_Scroll.Scroll4));
                Select_Flame_Image_18.SetActive(scroll.Equals(Stage_Scroll.Scroll4));
                Select_Flame_Image_19.SetActive(scroll.Equals(Stage_Scroll.Scroll4));
                Select_Flame_Image_20.SetActive(scroll.Equals(Stage_Scroll.Scroll4));
                Select_Flame_Image_21.SetActive(scroll.Equals(Stage_Scroll.Scroll5));
                Select_Flame_Image_22.SetActive(scroll.Equals(Stage_Scroll.Scroll5));
                Select_Flame_Image_23.SetActive(scroll.Equals(Stage_Scroll.Scroll5));
                Select_Flame_Image_24.SetActive(scroll.Equals(Stage_Scroll.Scroll5));
                Select_Flame_Image_25.SetActive(scroll.Equals(Stage_Scroll.Scroll5));
                Select_Flame_Image_26.SetActive(scroll.Equals(Stage_Scroll.Scroll6));
                Select_Flame_Image_27.SetActive(scroll.Equals(Stage_Scroll.Scroll6));
                Select_Flame_Image_28.SetActive(scroll.Equals(Stage_Scroll.Scroll6));
                Select_Flame_Image_29.SetActive(scroll.Equals(Stage_Scroll.Scroll6));
                Select_Flame_Image_30.SetActive(scroll.Equals(Stage_Scroll.Scroll6));
                Select_Flame_Image_31.SetActive(scroll.Equals(Stage_Scroll.Scroll7));
                Select_Flame_Image_32.SetActive(scroll.Equals(Stage_Scroll.Scroll7));
                Select_Flame_Image_33.SetActive(scroll.Equals(Stage_Scroll.Scroll7));
                Select_Flame_Image_34.SetActive(scroll.Equals(Stage_Scroll.Scroll7));
                Select_Flame_Image_35.SetActive(scroll.Equals(Stage_Scroll.Scroll7));
                Select_Flame_Image_36.SetActive(scroll.Equals(Stage_Scroll.Scroll8));
                Select_Flame_Image_37.SetActive(scroll.Equals(Stage_Scroll.Scroll8));
                Select_Flame_Image_38.SetActive(scroll.Equals(Stage_Scroll.Scroll8));
                Select_Flame_Image_39.SetActive(scroll.Equals(Stage_Scroll.Scroll8));
                Select_Flame_Image_40.SetActive(scroll.Equals(Stage_Scroll.Scroll8));
                Select_Flame_Image_41.SetActive(scroll.Equals(Stage_Scroll.Scroll9));
                Select_Flame_Image_42.SetActive(scroll.Equals(Stage_Scroll.Scroll9));
                Select_Flame_Image_43.SetActive(scroll.Equals(Stage_Scroll.Scroll9));
                Select_Flame_Image_44.SetActive(scroll.Equals(Stage_Scroll.Scroll9));
                Select_Flame_Image_45.SetActive(scroll.Equals(Stage_Scroll.Scroll9));
                Select_Flame_Image_46.SetActive(scroll.Equals(Stage_Scroll.Scroll10));
                Select_Flame_Image_47.SetActive(scroll.Equals(Stage_Scroll.Scroll10));
                Select_Flame_Image_48.SetActive(scroll.Equals(Stage_Scroll.Scroll10));
                Select_Flame_Image_49.SetActive(scroll.Equals(Stage_Scroll.Scroll10));
                Select_Flame_Image_50.SetActive(scroll.Equals(Stage_Scroll.Scroll10));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        // Update is called once per frame
        async public void Update()
        {
            switch (scroll)
            {
                case Stage_Scroll.Scroll1:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(false);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage1:
                            select_stage_pos.x = select_flame_image_01_pos.x;
                            select_stage_pos.y = select_flame_image_01_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage1);
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage2;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if(select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage2:
                            select_stage_pos.x = select_flame_image_02_pos.x;
                            select_stage_pos.y = select_flame_image_02_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage2);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage1;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage3;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage3:
                            select_stage_pos.x = select_flame_image_03_pos.x;
                            select_stage_pos.y = select_flame_image_03_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage3);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage2;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage4;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage4:
                            select_stage_pos.x = select_flame_image_04_pos.x;
                            select_stage_pos.y = select_flame_image_04_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage4);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage3;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage5;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage5:
                            select_stage_pos.x = select_flame_image_05_pos.x;
                            select_stage_pos.y = select_flame_image_05_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage5);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage4;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll2;
                                stage = Stage_Num.Stage6;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll2:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage6:
                            select_stage_pos.x = select_flame_image_06_pos.x;
                            select_stage_pos.y = select_flame_image_06_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage6);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll1;
                                stage = Stage_Num.Stage5;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage7;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage7:
                            select_stage_pos.x = select_flame_image_07_pos.x;
                            select_stage_pos.y = select_flame_image_07_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage7);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage6;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage8;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage8:
                            select_stage_pos.x = select_flame_image_08_pos.x;
                            select_stage_pos.y = select_flame_image_08_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage8);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage7;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage9;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage9:
                            select_stage_pos.x = select_flame_image_09_pos.x;
                            select_stage_pos.y = select_flame_image_09_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage9);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage8;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage10;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage10:
                            select_stage_pos.x = select_flame_image_10_pos.x;
                            select_stage_pos.y = select_flame_image_10_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage10);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage9;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll3;
                                stage = Stage_Num.Stage11;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll3:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage11:
                            select_stage_pos.x = select_flame_image_11_pos.x;
                            select_stage_pos.y = select_flame_image_11_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage11);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll2;
                                stage = Stage_Num.Stage10;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage12;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage12:
                            select_stage_pos.x = select_flame_image_12_pos.x;
                            select_stage_pos.y = select_flame_image_12_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage12);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage11;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage13;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage13:
                            select_stage_pos.x = select_flame_image_13_pos.x;
                            select_stage_pos.y = select_flame_image_13_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage13);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage12;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage14;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage14:
                            select_stage_pos.x = select_flame_image_14_pos.x;
                            select_stage_pos.y = select_flame_image_14_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage14);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage13;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage15;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage15:
                            select_stage_pos.x = select_flame_image_15_pos.x;
                            select_stage_pos.y = select_flame_image_15_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage15);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage14;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll4;
                                stage = Stage_Num.Stage16;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll4:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage16:
                            select_stage_pos.x = select_flame_image_16_pos.x;
                            select_stage_pos.y = select_flame_image_16_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage16);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll3;
                                stage = Stage_Num.Stage15;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage17;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage17:
                            select_stage_pos.x = select_flame_image_17_pos.x;
                            select_stage_pos.y = select_flame_image_17_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage17);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage16;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage18;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage18:
                            select_stage_pos.x = select_flame_image_18_pos.x;
                            select_stage_pos.y = select_flame_image_18_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage18);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage17;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage19;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage19:
                            select_stage_pos.x = select_flame_image_19_pos.x;
                            select_stage_pos.y = select_flame_image_19_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage19);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage18;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage20;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage20:
                            select_stage_pos.x = select_flame_image_20_pos.x;
                            select_stage_pos.y = select_flame_image_20_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage20);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage19;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll5;
                                stage = Stage_Num.Stage21;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll5:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage21:
                            select_stage_pos.x = select_flame_image_21_pos.x;
                            select_stage_pos.y = select_flame_image_21_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage21);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll4;
                                stage = Stage_Num.Stage20;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage22;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage22:
                            select_stage_pos.x = select_flame_image_22_pos.x;
                            select_stage_pos.y = select_flame_image_22_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage22);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage21;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage23;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage23:
                            select_stage_pos.x = select_flame_image_23_pos.x;
                            select_stage_pos.y = select_flame_image_23_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage23);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage22;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage24;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage24:
                            select_stage_pos.x = select_flame_image_24_pos.x;
                            select_stage_pos.y = select_flame_image_24_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage24);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage23;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage25;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage25:
                            select_stage_pos.x = select_flame_image_25_pos.x;
                            select_stage_pos.y = select_flame_image_25_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage25);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage24;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll6;
                                stage = Stage_Num.Stage26;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll6:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage26:
                            select_stage_pos.x = select_flame_image_26_pos.x;
                            select_stage_pos.y = select_flame_image_26_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage26);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll5;
                                stage = Stage_Num.Stage25;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage27;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage27:
                            select_stage_pos.x = select_flame_image_27_pos.x;
                            select_stage_pos.y = select_flame_image_27_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage27);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage26;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage28;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage28:
                            select_stage_pos.x = select_flame_image_28_pos.x;
                            select_stage_pos.y = select_flame_image_28_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage28);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage27;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage29;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage29:
                            select_stage_pos.x = select_flame_image_29_pos.x;
                            select_stage_pos.y = select_flame_image_29_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage29);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage28;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage30;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage30:
                            select_stage_pos.x = select_flame_image_30_pos.x;
                            select_stage_pos.y = select_flame_image_30_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage30);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage29;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll7;
                                stage = Stage_Num.Stage31;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll7:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage31:
                            select_stage_pos.x = select_flame_image_31_pos.x;
                            select_stage_pos.y = select_flame_image_31_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage31);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll6;
                                stage = Stage_Num.Stage30;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage32;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage32:
                            select_stage_pos.x = select_flame_image_32_pos.x;
                            select_stage_pos.y = select_flame_image_32_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage32);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage31;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage33;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage33:
                            select_stage_pos.x = select_flame_image_33_pos.x;
                            select_stage_pos.y = select_flame_image_33_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage33);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage32;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage34;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage34:
                            select_stage_pos.x = select_flame_image_34_pos.x;
                            select_stage_pos.y = select_flame_image_34_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage34);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage33;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage35;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage35:
                            select_stage_pos.x = select_flame_image_35_pos.x;
                            select_stage_pos.y = select_flame_image_35_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage35);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage34;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll8;
                                stage = Stage_Num.Stage36;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll8:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage36:
                            select_stage_pos.x = select_flame_image_36_pos.x;
                            select_stage_pos.y = select_flame_image_36_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage36);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll7;
                                stage = Stage_Num.Stage35;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage37;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage37:
                            select_stage_pos.x = select_flame_image_37_pos.x;
                            select_stage_pos.y = select_flame_image_37_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage37);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage36;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage38;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage38:
                            select_stage_pos.x = select_flame_image_38_pos.x;
                            select_stage_pos.y = select_flame_image_38_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage38);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage37;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage39;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage39:
                            select_stage_pos.x = select_flame_image_39_pos.x;
                            select_stage_pos.y = select_flame_image_39_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage39);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage38;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage40;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage40:
                            select_stage_pos.x = select_flame_image_40_pos.x;
                            select_stage_pos.y = select_flame_image_40_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage40);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage39;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll9;
                                stage = Stage_Num.Stage41;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll9:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(true);
                    switch (stage)
                    {
                        case Stage_Num.Stage41:
                            select_stage_pos.x = select_flame_image_41_pos.x;
                            select_stage_pos.y = select_flame_image_41_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage41);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll8;
                                stage = Stage_Num.Stage40;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage42;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage42:
                            select_stage_pos.x = select_flame_image_42_pos.x;
                            select_stage_pos.y = select_flame_image_42_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage42);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage41;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage43;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage43:
                            select_stage_pos.x = select_flame_image_43_pos.x;
                            select_stage_pos.y = select_flame_image_43_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage43);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage42;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage44;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage44:
                            select_stage_pos.x = select_flame_image_44_pos.x;
                            select_stage_pos.y = select_flame_image_44_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage44);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage43;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage45;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage45:
                            select_stage_pos.x = select_flame_image_45_pos.x;
                            select_stage_pos.y = select_flame_image_45_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage45);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage44;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                scroll = Stage_Scroll.Scroll10;
                                stage = Stage_Num.Stage46;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;

                case Stage_Scroll.Scroll10:
                    RenderUpdateSelectFlameImage(scroll);
                    Select_Left_Arrow.SetActive(true);
                    Select_Right_Arrow.SetActive(false);
                    switch (stage)
                    {
                        case Stage_Num.Stage46:
                            select_stage_pos.x = select_flame_image_46_pos.x;
                            select_stage_pos.y = select_flame_image_46_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage46);
                            if (GetInputLeft())
                            {
                                scroll = Stage_Scroll.Scroll9;
                                stage = Stage_Num.Stage45;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage47;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage47:
                            select_stage_pos.x = select_flame_image_47_pos.x;
                            select_stage_pos.y = select_flame_image_47_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage47);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage46;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage48;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage48:
                            select_stage_pos.x = select_flame_image_48_pos.x;
                            select_stage_pos.y = select_flame_image_48_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage48);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage47;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage49;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage49:
                            select_stage_pos.x = select_flame_image_49_pos.x;
                            select_stage_pos.y = select_flame_image_49_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage49);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage48;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                stage = Stage_Num.Stage50;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;

                        case Stage_Num.Stage50:
                            select_stage_pos.x = select_flame_image_50_pos.x;
                            select_stage_pos.y = select_flame_image_50_pos.y;
                            Select_Stage_Frame.GetComponent<RectTransform>().anchoredPosition = select_stage_pos;
                            GameObject.Find("Select_Comment_Image").GetComponent<Select_Comment>().Comment((int)Stage_Num.Stage50);
                            if (GetInputLeft())
                            {
                                stage = Stage_Num.Stage49;
                                SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputRight())
                            {
                                // 最終ステージ
                                //scroll = Stage_Scroll.Scroll10;
                                //stage = Stage_Num.Stage46;
                                //SfxPlay.Instance.PlaySFX(ClipToPlay.se_select);
                            }
                            if (GetInputSubmit())
                            {
                                if (select_decide == false)
                                {
                                    select_decide = true;
                                    Select_SceneToMainScene((int)stage - 1);
                                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_decided);
                                }
                            }
                            break;
                    }
                    break;
            }

            if (GetInputCancel())
            {
                if(select_cancel == false)
                {
                    select_cancel = true;
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                    GameObject.Find("FadeInOutPanel").GetComponent<FadeInOut>().Fadeout();
                    await Task.Delay(3000);
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }

        /// <summary>
        /// セレクトシーンからメインシーンへの遷移
        /// Select_Sceneから呼び出される想定の処理
        /// </summary>
        /// <param name="sceneId"></param>
        public void Select_SceneToMainScene(int sceneId)
        {
            BrideScenes_SelectMain.Instance.SetMainSceneNameIdFromSelect_Scene(sceneId);
            Observable.FromCoroutine<bool>(observer => GameObject.Find("FadeInOutPanel").GetComponent<FadeInOut>().Fadeout(observer))
                .Subscribe(_ => BrideScenes_SelectMain.Instance.PlayLoadScene())
                .AddTo(gameObject);
        }

        public bool GetInputCancel()
        {
            return Keyboard.current.backspaceKey.wasPressedThisFrame ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
        }

        public bool GetInputLeft()
        {
            return Keyboard.current.leftArrowKey.wasPressedThisFrame ||
                (Gamepad.current != null && Gamepad.current.leftStick.left.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.dpad.left.wasPressedThisFrame);
        }

        public bool GetInputRight()
        {
            return Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                (Gamepad.current != null && Gamepad.current.leftStick.right.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.dpad.right.wasPressedThisFrame);
        }

        public bool GetInputSubmit()
        {
            return Keyboard.current.enterKey.wasPressedThisFrame ||
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);
        }
    }

    /// <summary>
    /// InputSytemをInputAction未使用のままコードから直接呼び出す場合のインターフェース
    /// </summary>
    public interface IInputCallbackValue
    {
        /// <summary>
        /// 左入力
        /// </summary>
        /// <returns>入力されたらTrue</returns>
        public bool GetInputLeft();
        /// <summary>
        /// 右入力
        /// </summary>
        /// <returns>入力されたらTrue</returns>
        public bool GetInputRight();
        /// <summary>
        /// 決定入力
        /// </summary>
        /// <returns>入力されたらTrue</returns>
        public bool GetInputSubmit();
        /// <summary>
        /// キャンセル入力
        /// </summary>
        /// <returns>入力されたらTrue</returns>
        public bool GetInputCancel();
    }
}
