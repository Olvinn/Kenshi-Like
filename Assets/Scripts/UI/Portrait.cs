using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Portrait : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}
