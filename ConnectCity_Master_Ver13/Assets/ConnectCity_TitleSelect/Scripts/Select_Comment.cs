using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Trial.TitleSelect;

namespace TitleSelect
{
    public class Select_Comment : MonoBehaviour
    {
        public Text text;
        /// <summary>体験版セレクトシーンの制御</summary>
        private InputKey_SelectTrial _inputKey_SelectTrial;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Comment(int stage)
        {
            if (_inputKey_SelectTrial == null) { }
                _inputKey_SelectTrial = GameObject.Find("GameObject").GetComponent<InputKey_SelectTrial>();
            if (_inputKey_SelectTrial.TrialMode)
                stage = _inputKey_SelectTrial.GetStageNumberTrial(stage);
            if (stage < 1)
                throw new System.Exception("ステージ番号取得の失敗");
            if (stage == 1)
            {
                text.text = "難易度【★☆☆☆☆☆☆☆☆☆】\r\nコネクトシティでの基本操作を覚える\r\nステージだよ。";
            }

            if (stage == 2)
            {
                text.text = "難易度【★☆☆☆☆☆☆☆☆☆】\r\nコネクトシティでのコネクトや空間操作\r\nの仕組みを覚えるステージだよ。";
            }

            if (stage == 3)
            {
                text.text = "難易度【★☆☆☆☆☆☆☆☆☆】\r\n空間操作でブロックの再利用ができる事\r\nを知ってもらうステージだよ。";
            }

            if (stage == 4)
            {
                text.text = "難易度【★☆☆☆☆☆☆☆☆☆】\r\n左右同時に空間操作して攻略する\r\nステージになるよ。";
            }

            if (stage == 5)
            {
                text.text = "難易度【★☆☆☆☆☆☆☆☆☆】\r\n総復習ステージ�@\r\nこれまで実践した操作を使って\r\n攻略するステージになるよ。";
            }

            if (stage == 6)
            {
                text.text = "難易度【★★☆☆☆☆☆☆☆☆】\r\n空間操作をメインにこれまで実践した操作で攻略するステージになるよ。";
            }

            if (stage == 7)
            {
                text.text = "難易度【★★☆☆☆☆☆☆☆☆】\r\nプレイヤーを落とさないように\r\n空間操作を利用して攻略するステージだよ。";
            }

            if (stage == 8)
            {
                text.text = "難易度【★★☆☆☆☆☆☆☆☆】\r\nコネクトする順番を考えないと\r\nクリアする事ができないステージだよ";
            }

            if (stage == 9)
            {
                text.text = "難易度【★★☆☆☆☆☆☆☆☆】\r\n空間操作とコネクトする順番を考えないと\r\nクリアする事ができないステージだよ";
            }

            if (stage == 10)
            {
                text.text = "難易度【★★☆☆☆☆☆☆☆☆】\r\n総復習ステージ�A\r\nこれまで実践した操作を使って\r\n攻略するステージになるよ。";
            }

            if (stage == 11)
            {
                text.text = "難易度【★★★☆☆☆☆☆☆☆】\r\n新ギミック 「敵」が登場\r\n空間操作を使って攻略する事が鍵になるよ。";
            }

            if (stage == 12)
            {
                text.text = "難易度【★★★☆☆☆☆☆☆☆】\r\n敵をいかに捌く事ができるかが\r\n攻略の鍵になるよ。";
            }

            if (stage == 13)
            {
                text.text = "難易度【★★★☆☆☆☆☆☆☆】\r\n空間操作とコネクトを工夫して、\r\nゴールする必要があるよ。";
            }

            if (stage == 14)
            {
                text.text = "難易度【★★★☆☆☆☆☆☆☆】\r\n新ギミック 「ぼろいブロック・天井」が登場。\r\n空間操作でぼろいブロックや天井を壊そう";
            }

            if (stage == 15)
            {
                text.text = "難易度【★★★☆☆☆☆☆☆☆】\r\nぼろいブロックを壊しつつ、\r\nゴールをステージ外に落とさないようにする工夫も必要になるよ";
            }

            if (stage == 16)
            {
                text.text = "難易度【★★★★☆☆☆☆☆☆】\r\n自身の足場を残さないと\r\n攻略できないステージだよ";
            }

            if (stage == 17)
            {
                text.text = "難易度【★★★★☆☆☆☆☆☆】\r\n新ギミック 「重力」が登場\r\n空間操作を使って\r\n重力を攻略する事が鍵になるよ。";
            }

            if (stage == 18)
            {
                text.text = "難易度【★★★★☆☆☆☆☆☆】\r\n重力の流れを見て、空間操作とコネクトを駆使する事が攻略の鍵になるよ。";
            }

            if (stage == 19)
            {
                text.text = "難易度【★★★★☆☆☆☆☆☆】\r\n重力とぼろいブロックで落ちないように\r\n気を付けてコネクトする必要があるよ。";
            }

            if (stage == 20)
            {
                text.text = "難易度【★★★★☆☆☆☆☆☆】\r\n総復習ステージ�B\r\n今まで登場したギミックが全て出るよ\r\nこれまでの操作を活かして攻略しよう。";
            }

            if (stage == 21)
            {
                text.text = "難易度【★★★★★☆☆☆☆☆】\r\n新ギミック 「レーザー」が登場\r\nレーザーを回避して攻略しよう。";
            }

            if (stage == 22)
            {
                text.text = "難易度【★★★★★☆☆☆☆☆】\r\nプレイヤーを「レーザー」から守るように\r\nコネクトする事が攻略の鍵だよ。";
            }

            if (stage == 23)
            {
                text.text = "難易度【★★★★★☆☆☆☆☆】\r\nレーザーの攻略とゴールをステージ外に\r\n落とさないよう工夫が必要だよ。";
            }

            if (stage == 24)
            {
                text.text = "難易度【★★★★★☆☆☆☆☆】\r\n空間操作したブロックがステージ外に\r\n行かないようにする工夫が必要だよ。";
            }

            if (stage == 25)
            {
                text.text = "難易度【★★★★★☆☆☆☆☆】\r\n重力と空間操作に気を付けて\r\n攻略する必要があるよ。";
            }

            if (stage == 26)
            {
                text.text = "難易度【★★★★★★☆☆☆☆】\r\n新ギミック 「ワープ」が登場\r\nワープの仕組みを理解して攻略しよう。";
            }

            if (stage == 27)
            {
                text.text = "難易度【★★★★★★☆☆☆☆】\r\nワープは敵にも有効となる為、\r\n敵と遭遇しないように工夫する必要がある";
            }

            if (stage == 28)
            {
                text.text = "難易度【★★★★★★☆☆☆☆】\r\nワープするタイミングに気を付けつつ、\r\nコネクトする工夫が必要となるステージだよ";
            }

            if (stage == 29)
            {
                text.text = "難易度【★★★★★★☆☆☆☆】\r\n新ギミック 「ON_OFFブロック」登場\r\nON_OFFブロックの仕組みを理解して\r\n攻略しよう。";
            }

            if (stage == 30)
            {
                text.text = "難易度【★★★★★★☆☆☆☆】\r\n足場のほとんどが\r\nON_OFFブロックで構成したステージ\r\nON_OFFブロックを切り替えるタイミングが重要となる";
            }

            if (stage == 31) text.text = "難易度【★★★★★★★☆☆☆】\r\n新ギミック 「追尾ドローン」が登場\r\n追尾ドローンの仕組みを理解して攻略しよう";
            if (stage == 32) text.text = "難易度【★★★★★★★☆☆☆】\r\n追尾ドローンは各種オブジェクトを貫通する\r\nドローンを避けつつゴールを目指そう";
            if (stage == 33) text.text = "難易度【★★★★★★★☆☆☆】\r\nぼろいブロックを壊して\r\n追尾ドローンを避けつつ、\r\nゴールを目指そう";
            if (stage == 34) text.text = "難易度【★★★★★★★☆☆☆】\r\n新ギミック 「条件付きブロック」が登場\r\n条件付きブロックの仕組みを理解して\r\n攻略しよう。";
            if (stage == 35) text.text = "難易度【★★★★★★★☆☆☆】\r\nステージ２６〜３４まで登場した\r\n新ギミックを全て使用\r\n各種ギミックを駆使して攻略しよう";
            if (stage == 36) text.text = "難易度【★★★★★★★★☆☆】\r\nレーザーを防ぎつつ、空間操作とコネクト\r\nを応用して攻略する必要があるよ。";
            if (stage == 37) text.text = "難易度【★★★★★★★★☆☆】\r\nレーザー、足場に気を付けてつつ、\r\n空間操作とコネクトを応用して\r\n攻略するステージになるよ。";
            if (stage == 38) text.text = "難易度【★★★★★★★★☆☆】\r\n敵が数多く配置されており、\r\nワープを駆使して動き回る\r\nON_OFFブロックの切り替えに\r\n気を付けて攻略しよう";
            if (stage == 39) text.text = "難易度【★★★★★★★★☆☆】\r\n足場と空間操作に気を付けることが\r\nポイントだよ。";
            if (stage == 40) text.text = "難易度【★★★★★★★★★☆】\r\n足場と重力に気を付けつつ、\r\nコネクトする事が攻略の鍵だよ。";
            if (stage == 41) text.text = "難易度【★★★★★★★★★☆】\r\n足場と重力とレーザーに気を付けて\r\n攻略しよう";
            if (stage == 42) text.text = "難易度【★★★★★★★★★☆】\r\n地面が少なく、周囲に重力があるステージ\r\n追尾ドローンを避けながらコネクトをしよう";
            if (stage == 43) text.text = "難易度【★★★★★★★★★☆】\r\n空間操作で大量にブロックを崩して\r\n攻略しよう";
            if (stage == 44) text.text = "難易度【★★★★★★★★★☆】\r\nワープの開閉するタイミングが\r\n攻略の鍵となる";
            if (stage == 45) text.text = "難易度【★★★★★★★★★☆】\r\n迷路風ステージを作成\r\n敵とドローンを避けて攻略せよ";
            if (stage == 46) text.text = "難易度【★★★★★★★★★★】\r\n最難関ステージ�@\r\n空中戦セミファイナル";
            if (stage == 47) text.text = "難易度【★★★★★★★★★★】\r\n最難関ステージ�A\r\n大量のレーザーから身を守りつつゴールせよ";
            if (stage == 48) text.text = "難易度【★★★★★★★★★★】\r\n最難関ステージ�B\r\n大量の敵を捌く事が攻略の鍵になるステージ";
            if (stage == 49) text.text = "難易度【★★★★★★★★★★】\r\n最難関ステージ�C\r\n空中戦ファイナル";
            if (stage == 50) text.text = "難易度【★★★★★★★★★★】\r\n最難関ステージ�D\r\nこれまで出現したギミックが全て入っているステージ";
        }
    }
}
