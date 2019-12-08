using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace MaterialUI {
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
        public float CornerRadius {
            get { return CornerRadius; }
            set { SetCornerRadius (value); }
        }
        public Vector4 CornersRadius {
            get { return cornersRadius; }
            set { cornersRadius = value; SetCornersRadius (cornersRadius); }
        }
        public ProceduralImage imgContentBackground;
        public ProceduralImage imgShadow;
        public ProceduralImage imgFader;
        public Text txtContent;
        [SerializeField] private float cornerRadius;
        [SerializeField] private Vector4 cornersRadius;

        public UnityEvent onClick;

        #region LIFE CYCLE
        private void Start () {
            SetCornerRadius (cornerRadius);
        }
        #endregion

        #region EVENTS
        public void OnPointerEnter (PointerEventData eventData) {
            AnimatePointerEnter ();
        }

        public void OnPointerExit (PointerEventData eventData) {
            AnimatePointerExit ();
        }
        public void OnPointerDown (PointerEventData eventData) {
            AnimatePointerDown (eventData);
        }

        public void OnPointerUp (PointerEventData eventData) {
            AnimatePointerUp ();
            if (onClick != null) {
                onClick.Invoke ();
            }
        }
        #endregion

        #region IMAGE FUNCTION
        private void SetCornerRadius (float value) {
            imgShadow.GetComponent<FreeModifier> ().Radius = Vector4.one * value;
            imgContentBackground.GetComponent<FreeModifier> ().Radius = Vector4.one * value;
        }
        private void SetCornersRadius (Vector4 value) {
            imgShadow.GetComponent<FreeModifier> ().Radius = value;
            imgContentBackground.GetComponent<FreeModifier> ().Radius = value;
        }
        #endregion

        #region ANIMATION 
        private void AnimatePointerEnter () {
            float currentBottom = imgShadow.rectTransform.offsetMin.y;
            LeanTween.value (imgShadow.gameObject, currentBottom, -2, .5f).setOnUpdate ((float value) => {
                imgShadow.rectTransform.offsetMin = new Vector2 (imgShadow.rectTransform.offsetMin.x, value);
            });
        }
        private void AnimatePointerExit () {
            float currentBottom = imgShadow.rectTransform.offsetMin.y;
            LeanTween.value (imgShadow.gameObject, currentBottom, 0, .5f).setOnUpdate ((float value) => {
                imgShadow.rectTransform.offsetMin = new Vector2 (imgShadow.rectTransform.offsetMin.x, value);
            });
        }

        private void AnimatePointerDown (PointerEventData eventData) {
            Vector2 pointInRect = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle (imgContentBackground.rectTransform, eventData.position, eventData.pressEventCamera, out pointInRect)) {
                imgFader.rectTransform.anchoredPosition = pointInRect;
                AnimateShowFader ();
            }
            if (LeanTween.isTweening (imgContentBackground.gameObject)) {
                LeanTween.cancel (imgContentBackground.gameObject);
            }
            imgContentBackground.transform.LeanScale (Vector3.one * .98f, .2f);
        }
        private void AnimatePointerUp () {
            AnimateHideFader ();
            if (LeanTween.isTweening (imgContentBackground.gameObject)) {
                imgContentBackground.transform.LeanScale (Vector3.one, .2f).setDelay (.2f).setEaseInQuint ();
            } else {
                imgContentBackground.transform.LeanScale (Vector3.one, .2f).setEaseInQuint ();
            }

        }
        private void AnimateShowFader () {
            float maxSize = Mathf.Sqrt (Mathf.Pow (imgContentBackground.rectTransform.rect.width, 2) + Mathf.Pow (imgContentBackground.rectTransform.rect.height, 2));
            if (LeanTween.isTweening (imgFader.gameObject)) {
                LeanTween.cancel (imgFader.gameObject);
            }
            imgFader.rectTransform.LeanSize (Vector2.zero, 0);
            imgFader.rectTransform.LeanSize (Vector2.one * maxSize * 2, .5f);
        }
        private void AnimateHideFader () {
            if (LeanTween.isTweening (imgFader.gameObject)) {
                imgFader.rectTransform.LeanSize (Vector2.zero, .5f).setDelay (.5f).setEaseOutQuint ();
            } else {
                imgFader.rectTransform.LeanSize (Vector2.zero, .5f).setEaseOutQuint ();
            }
        }
        #endregion
    }
}