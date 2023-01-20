using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class Portrait : MonoBehaviour, IPoolable, IPointerClickHandler
    {
        public event Action onClick; 
        public event Action<IPoolable> onUnload;
        [SerializeField] private Image image;

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void Unload()
        {
            onClick = null;
            gameObject.SetActive(false);
            onUnload?.Invoke(this);
        }

        public void Load()
        {
            gameObject.SetActive(true);
        }

        public void Select()
        {
            
        }

        public void Deselect()
        {
            
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}
