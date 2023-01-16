using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WidgetView : MonoBehaviour
    {
        [SerializeField] private List<Image> accentColor, yesColor, noColor, ambientColor, backColor;

        public void UpdateColors(ColorsPreset preset)
        {
            foreach (var image in accentColor)
                image.color = preset.AccentColor;
            
            foreach (var image in yesColor)
                image.color = preset.AcceptColor;
            
            foreach (var image in noColor)
                image.color = preset.DeclineColor;
            
            foreach (var image in ambientColor)
                image.color = preset.AmbientColor;
            
            foreach (var image in backColor)
                image.color = preset.BackgroundColor;
        }
    }
}
