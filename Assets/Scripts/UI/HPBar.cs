using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Image hp;

        public void UpdateHp(float value)
        {
            hp.fillAmount = value;
        }
    }
}
