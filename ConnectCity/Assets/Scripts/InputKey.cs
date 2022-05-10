using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using Main.Common;

namespace TitleSelect
{
    public class InputKey : MonoBehaviour
    {
        public GameObject title_image;
        public GameObject push_image;
        public GameObject gamestart_image;
        public GameObject gamefinish_image;
        public GameObject pause_pencil_image;
        public GameObject gamefinish_check_image;
        public GameObject yes_image;
        public GameObject no_image;

        public Vector3 pencil_pos;
        public Vector3 gamestart_pos;
        public Vector3 gamefinish_pos;
        public Vector3 yes_pos;
        public Vector3 no_pos;

        public enum Title_Status
        {
            Push_Button,
            Start_End,
            End_Confirm
        }
        public enum Start_End
        {
            Start,
            End
        }
        public enum Yes_No
        {
            Yes,
            No
        }

        Title_Status status;
        Start_End start_end;
        Yes_No yes_no;
        // Start is called before the first frame update
        public void Start()
        {
            title_image = GameObject.Find("Title_Logo_Image");
            push_image = GameObject.Find("PushGameStart_Logo_Image");
            gamestart_image = GameObject.Find("GameStart_Logo_Image");
            gamefinish_image = GameObject.Find("GameFinish_Logo_Image");
            pause_pencil_image = GameObject.Find("Pause_Pencil_Logo_Image");
            gamefinish_check_image = GameObject.Find("GameFinish_Check_Logo_Image");
            yes_image = GameObject.Find("Yes_Logo_Image");
            no_image = GameObject.Find("No_Logo_Image");
            pencil_pos = pause_pencil_image.GetComponent<RectTransform>().anchoredPosition;
            gamestart_pos = gamestart_image.GetComponent<RectTransform>().anchoredPosition;
            gamefinish_pos = gamefinish_image.GetComponent<RectTransform>().anchoredPosition;
            yes_pos = yes_image.GetComponent<RectTransform>().anchoredPosition;
            no_pos = no_image.GetComponent<RectTransform>().anchoredPosition;
            status = Title_Status.Push_Button;
            push_image.SetActive(true);
            gamestart_image.SetActive(false);
            gamefinish_image.SetActive(false);
            pause_pencil_image.SetActive(false);
            gamefinish_check_image.SetActive(false);
            yes_image.SetActive(false);
            no_image.SetActive(false);
        }

        // Update is called once per frame
        async public void Update()
        {
            switch (status)
            {
                case Title_Status.Push_Button:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        push_image.SetActive(false);
                        gamestart_image.SetActive(true);
                        gamefinish_image.SetActive(true);
                        pause_pencil_image.SetActive(true);
                        start_end = Start_End.Start;
                        pencil_pos.x = gamestart_pos.x - 280;
                        pencil_pos.y = gamestart_pos.y;
                        pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                        status = Title_Status.Start_End;
                    }
                    break;

                case Title_Status.Start_End:
                    if (start_end == Start_End.Start)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            Debug.Log("下ボタン");
                            Debug.Log(pencil_pos.x);
                            pencil_pos.x = gamefinish_pos.x - 275;
                            pencil_pos.y = gamefinish_pos.y;
                            pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                            start_end = Start_End.End;
                        }
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            GameObject.Find("FadeInOutPanel").GetComponent<FadeInOut>().Fadeout();
                            await Task.Delay(3000);
                            SceneManager.LoadScene("SelectScene");
                        }
                    }
                    else if (start_end == Start_End.End)
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            Debug.Log("上ボタン");
                            Debug.Log(pencil_pos.x);
                            pencil_pos.x = gamestart_pos.x - 280;
                            pencil_pos.y = gamestart_pos.y;
                            pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                            start_end = Start_End.Start;
                        }
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            gamestart_image.SetActive(false);
                            gamefinish_image.SetActive(false);
                            gamefinish_check_image.SetActive(true);
                            yes_image.SetActive(true);
                            no_image.SetActive(true);
                            yes_no = Yes_No.Yes;
                            pencil_pos.x = yes_pos.x - 100;
                            pencil_pos.y = yes_pos.y;
                            pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                            status = Title_Status.End_Confirm;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        gamestart_image.SetActive(false);
                        gamefinish_image.SetActive(false);
                        pause_pencil_image.SetActive(false);
                        push_image.SetActive(true);
                        status = Title_Status.Push_Button;
                    }
                    break;

                case Title_Status.End_Confirm:
                    if (yes_no == Yes_No.Yes)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            pencil_pos.x = no_pos.x - 180;
                            pencil_pos.y = no_pos.y;
                            pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                            yes_no = Yes_No.No;
                        }
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            Quit();
                        }
                    }
                    else if (yes_no == Yes_No.No)
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            pencil_pos.x = yes_pos.x - 100;
                            pencil_pos.y = yes_pos.y;
                            pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                            yes_no = Yes_No.Yes;
                        }
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            gamefinish_check_image.SetActive(false);
                            yes_image.SetActive(false);
                            no_image.SetActive(false);
                            gamestart_image.SetActive(true);
                            gamefinish_image.SetActive(true);
                            start_end = Start_End.Start;
                            pencil_pos.x = gamestart_pos.x - 280;
                            pencil_pos.y = gamestart_pos.y;
                            pause_pencil_image.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                            status = Title_Status.Start_End;
                        }
                    }
                    break;
            }
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif

        }
    }
}
