using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using Main.Common;
using UniRx;
using UniRx.Triggers;

namespace TitleSelect
{
    /// <summary>
    /// ëÄçÏÉLÅ[ÇÃêßå‰
    /// </summary>
    public class InputKey : MonoBehaviour
    {
        //[SerializeField] private GameObject title_image;
        [SerializeField] private GameObject pushImage;
        [SerializeField] private GameObject gamestartImage;
        [SerializeField] private GameObject gamefinishImage;
        [SerializeField] private GameObject pause_pencilImage;
        [SerializeField] private GameObject gamefinishCheckImage;
        [SerializeField] private GameObject yesImage;
        [SerializeField] private GameObject noImage;

        //public enum Start_End
        //{
        //    Start,
        //    End
        //}
        //public enum Yes_No
        //{
        //    Yes,
        //    No
        //}

        private void Reset()
        {
            //title_image = GameObject.Find("Title_Logo_Image");
            pushImage = GameObject.Find("PushGameStart_Logo_Image");
            gamestartImage = GameObject.Find("GameStart_Logo_Image");
            gamefinishImage = GameObject.Find("GameFinish_Logo_Image");
            pause_pencilImage = GameObject.Find("Pause_Pencil_Logo_Image");
            gamefinishCheckImage = GameObject.Find("GameFinish_Check_Logo_Image");
            yesImage = GameObject.Find("Yes_Logo_Image");
            noImage = GameObject.Find("No_Logo_Image");
        }

        private void Start()
        {
            Vector3 pencil_pos = pause_pencilImage.GetComponent<RectTransform>().anchoredPosition;
            Vector3 gamestart_pos = gamestartImage.GetComponent<RectTransform>().anchoredPosition;
            Vector3 gamefinish_pos = gamefinishImage.GetComponent<RectTransform>().anchoredPosition;
            Vector3 yes_pos = yesImage.GetComponent<RectTransform>().anchoredPosition;
            Vector3 no_pos = noImage.GetComponent<RectTransform>().anchoredPosition;
            var status = Title_Status.Push_Button;
            var startEnd = false;
            var yesNo = false;

            Cursor.visible = false;

            pushImage.SetActive(true);
            gamestartImage.SetActive(false);
            gamefinishImage.SetActive(false);
            pause_pencilImage.SetActive(false);
            gamefinishCheckImage.SetActive(false);
            yesImage.SetActive(false);
            noImage.SetActive(false);

            this.UpdateAsObservable()
                .Subscribe(async _ =>
                {
                    switch (status)
                    {
                        case Title_Status.Push_Button:
                            // èCê≥
                            if (Input.GetKeyDown(KeyCode.Space))
                            {
                                pushImage.SetActive(false);
                                gamestartImage.SetActive(true);
                                gamefinishImage.SetActive(true);
                                pause_pencilImage.SetActive(true);
                                startEnd = true;
                                pencil_pos.x = gamestart_pos.x - 280;
                                pencil_pos.y = gamestart_pos.y;
                                pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                status = Title_Status.Start_End;
                            }
                            break;

                        case Title_Status.Start_End:
                            // èCê≥
                            if (startEnd)
                            {
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.DownArrow))
                                {
                                    Debug.Log("â∫É{É^Éì");
                                    Debug.Log(pencil_pos.x);
                                    pencil_pos.x = gamefinish_pos.x - 275;
                                    pencil_pos.y = gamefinish_pos.y;
                                    pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                    startEnd = false;
                                }
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.Space))
                                {
                                    GameObject.Find("FadeInOutPanel").GetComponent<FadeInOut>().Fadeout();
                                    await Task.Delay(3000);
                                    SceneManager.LoadScene("SelectScene");
                                }
                            }
                            else if (!startEnd)
                            {
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.UpArrow))
                                {
                                    Debug.Log("è„É{É^Éì");
                                    Debug.Log(pencil_pos.x);
                                    pencil_pos.x = gamestart_pos.x - 280;
                                    pencil_pos.y = gamestart_pos.y;
                                    pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                    startEnd = true;
                                }
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.Space))
                                {
                                    gamestartImage.SetActive(false);
                                    gamefinishImage.SetActive(false);
                                    gamefinishCheckImage.SetActive(true);
                                    yesImage.SetActive(true);
                                    noImage.SetActive(true);
                                    yesNo = true;
                                    pencil_pos.x = yes_pos.x - 100;
                                    pencil_pos.y = yes_pos.y;
                                    pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                    status = Title_Status.End_Confirm;
                                }
                            }
                            // èCê≥
                            if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                gamestartImage.SetActive(false);
                                gamefinishImage.SetActive(false);
                                pause_pencilImage.SetActive(false);
                                pushImage.SetActive(true);
                                status = Title_Status.Push_Button;
                            }
                            break;

                        case Title_Status.End_Confirm:
                            if (yesNo)
                            {
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.DownArrow))
                                {
                                    pencil_pos.x = no_pos.x - 180;
                                    pencil_pos.y = no_pos.y;
                                    pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                    yesNo = false;
                                }
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.Space))
                                {
                                    Quit();
                                }
                            }
                            else if (!yesNo)
                            {
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.UpArrow))
                                {
                                    pencil_pos.x = yes_pos.x - 100;
                                    pencil_pos.y = yes_pos.y;
                                    pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                    yesNo = true;
                                }
                                // èCê≥
                                if (Input.GetKeyDown(KeyCode.Space))
                                {
                                    gamefinishCheckImage.SetActive(false);
                                    yesImage.SetActive(false);
                                    noImage.SetActive(false);
                                    gamestartImage.SetActive(true);
                                    gamefinishImage.SetActive(true);
                                    startEnd = true;
                                    pencil_pos.x = gamestart_pos.x - 280;
                                    pencil_pos.y = gamestart_pos.y;
                                    pause_pencilImage.GetComponent<RectTransform>().anchoredPosition = pencil_pos;
                                    status = Title_Status.Start_End;
                                }
                            }
                            break;
                    }
                });
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif

        }
    }
    public enum Title_Status
    {
        Push_Button,
        Start_End,
        End_Confirm
    }
}
