using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

namespace TitleSelect
{
    public class FadeInOut : MonoBehaviour
    {
        [SerializeField] private Image background_image;
        [SerializeField] private float duration = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
            Fadein();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Fadein()
        {
            this.background_image.DOFade(endValue: 0f, duration: duration);
        }

        public void Fadeout()
        {
            this.background_image.DOFade(endValue: 1f, duration: duration);
        }

        public IEnumerator Fadeout(System.IObserver<bool> observer)
        {
            background_image.DOFade(endValue: 1f, duration: duration)
                .SetUpdate(true)
                .OnComplete(() => observer.OnNext(true));
            yield return null;
        }
    }
}
