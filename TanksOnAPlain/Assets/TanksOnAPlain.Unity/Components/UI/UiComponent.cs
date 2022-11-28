using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.UI
{
    public class UiComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<CanvasGroup> CanvasGroupsToShow { get; set; }
        
        [OdinSerialize]
        List<CanvasGroup> CanvasGroupsToHide { get; set; }
        
        [OdinSerialize]
        CanvasGroup CanvasGroupStartScreen { get; set; }

        public void ShowGameOver()
        {
            foreach (var canvasGroup in CanvasGroupsToShow)
            {
                EnableCanvasGroup(canvasGroup);
            }
            
            foreach (var canvasGroup in CanvasGroupsToHide)
            {
                DisableCanvasGroup(canvasGroup);
            }
        }

        public void StartGame()
        {
            Time.timeScale = 1;
            DisableCanvasGroup(CanvasGroupStartScreen);
        }
        
        void EnableCanvasGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        void DisableCanvasGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}