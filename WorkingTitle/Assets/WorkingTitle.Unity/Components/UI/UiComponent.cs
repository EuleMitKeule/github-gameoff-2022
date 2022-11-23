using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components.Health;

namespace WorkingTitle.Unity.Components.UI
{
    public class UiComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<CanvasGroup> CanvasGroupsToShow { get; set; }
        
        [OdinSerialize]
        List<CanvasGroup> CanvasGroupsToHide { get; set; }

        public void ShowGameOver()
        {
            foreach (var canvasGroup in CanvasGroupsToShow)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            foreach (var canvasGroup in CanvasGroupsToHide)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}