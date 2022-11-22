using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components.Health;

namespace WorkingTitle.Unity.Components.UI
{
    public class UiComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        GameObject OverlayGameOver { get; set; }

        public void ShowGameOver()
        {
            OverlayGameOver.SetActive(true);
        }

        public void OnButtonOkClick()
        {
            Debug.Log("sed");
        }
    }
}