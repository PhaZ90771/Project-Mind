using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace PicoGames.Panes
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPane : MonoBehaviour
    {
        public bool accessableToPanesClass = false;
        public string identity = "";
        public float fadeInTime = 0.25f;
        public float fadeOutTime = 0.25f;
        public bool startHidden = true;

        public UnityEvent onShow;
        public UnityEvent onHide;

        private CanvasGroup canvas;
        private Coroutine fadeRoutine;
        private bool isShowing = false;

        private void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
            isShowing = gameObject.activeSelf;

            if (startHidden)
                HideFast();
            else
                ShowFast();
        }
        
        public void Show()
        {
            Show(fadeInTime);
        }

        public void Show(float time)
        {
            if (isShowing)
                return;

            gameObject.SetActive(true);
            //frontPane.Add(identity);

            if (onShow != null)
                onShow.Invoke();

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            else
                canvas.alpha = 0;

            fadeRoutine = StartCoroutine(FadeAlpha(1f, time));
        }

        public void ShowFast()
        {
            if (isShowing)
                return;

            //frontPane.Remove(identity);
            fadeRoutine = null;

            if (onShow != null)
                onShow.Invoke();

            isShowing = true;
            canvas.alpha = 1;
            canvas.blocksRaycasts = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Hide(fadeOutTime);
        }

        public void Hide(float time)
        {
            if (!isShowing)
                return;

            //frontPane.Remove(identity);

            if (onHide != null)
                onHide.Invoke();

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            else
                canvas.alpha = 1f;

            fadeRoutine = StartCoroutine(FadeAlpha(0f, time));
        }

        public void HideFast()
        {
            if (!isShowing)
                return;

            //frontPane.Remove(identity);
            fadeRoutine = null;

            if (onHide != null)
                onHide.Invoke();

            isShowing = false;
            canvas.alpha = 0;
            canvas.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private IEnumerator FadeAlpha(float targetAlpha, float seconds)
        {
            isShowing = (targetAlpha > 0);
            canvas.interactable = false;

            if (isShowing)
                gameObject.SetActive(true);

            var startAlpha = canvas.alpha;
            var t = 0f;

            while(t < 1f)
            {
                t += Time.deltaTime / seconds;
                canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return 0;
            }
            canvas.alpha = targetAlpha;

            canvas.interactable = isShowing;
            canvas.blocksRaycasts = isShowing;

            if (!isShowing)
                gameObject.SetActive(false);

            fadeRoutine = null;
        }
    }
}