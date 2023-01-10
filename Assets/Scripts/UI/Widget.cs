using System.Collections;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Common class for any complex UI object
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Widget : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField, Tooltip("Should this widget catch raycasts and process it")] private bool isRaycastable = true;
        
        [Header("Manual serializing")]
        [SerializeField, Tooltip("You should assign cg manually only if widget have to do something on Awake")] protected CanvasGroup cg;
        [field: SerializeField] public bool isShown { get; private set; }
        
        private Coroutine _showing;

        protected virtual void Awake()
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
        }
        
        public virtual void ShowInstantly()
        {
            if (cg == null)
                return;
            isShown = true;
            if (_showing != null)
                StopCoroutine(_showing);
            cg.blocksRaycasts = isRaycastable;
            cg.interactable = isRaycastable;
            cg.alpha = 1;
        }
        
        public virtual void HideInstantly()
        {
            if (cg == null)
                return;
            isShown = false;
            if (_showing != null)
                StopCoroutine(_showing);
            cg.blocksRaycasts = false;
            cg.interactable = false;
            cg.alpha = 0;
        }

        public virtual void Show(float speed = .2f, System.Action callback = null)
        {
            if (cg == null)
                return;
            if (speed <= 0)
            {
                ShowInstantly();
                callback?.Invoke();
            }
            else
            {
                isShown = true;
                if (_showing != null)
                    StopCoroutine(_showing);
                cg.blocksRaycasts = isRaycastable;
                cg.interactable = isRaycastable;
                if (gameObject.activeSelf)
                    _showing = StartCoroutine(LerpAlpha(1f, speed, callback));
            }
        }

        public virtual void Hide(float speed = .2f, System.Action callback = null)
        {
            if (cg == null)
                return;
            if (speed <= 0)
            {
                HideInstantly();
                callback?.Invoke();
            }
            else
            {
                isShown = false;
                if (_showing != null)
                    StopCoroutine(_showing);
                cg.blocksRaycasts = false;
                cg.interactable = false;
                if (gameObject.activeSelf)
                    _showing = StartCoroutine(LerpAlpha(0f, speed, callback));
            }
        }

        public virtual void Toggle(float speed = .2f)
        {
            if (cg == null)
                return;
            isShown = !isShown;
            if (isShown)
                Show();
            else
                Hide();
        }

        IEnumerator LerpAlpha(float alpha, float speed, System.Action callback = null)
        {
            if (speed != 0)
                while (alpha != cg.alpha)
                {
                    if (alpha > cg.alpha)
                    {
                        cg.alpha = cg.alpha + Time.unscaledDeltaTime / speed;
                        if (cg.alpha >= alpha)
                            cg.alpha = alpha;
                    }
                    else
                    {
                        cg.alpha = cg.alpha - Time.unscaledDeltaTime / speed;
                        if (cg.alpha <= alpha)
                            cg.alpha = alpha;
                    }
                    yield return null;
                }
            else
                cg.alpha = alpha;
            
            callback?.Invoke();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
            if (isShown)
            {
                cg.blocksRaycasts = isRaycastable;
                cg.interactable = isRaycastable;
                cg.alpha = 1;
            }
            else
            {
                cg.blocksRaycasts = false;
                cg.interactable = false;
                cg.alpha = 0;
            }
#endif
        }
    }
}
