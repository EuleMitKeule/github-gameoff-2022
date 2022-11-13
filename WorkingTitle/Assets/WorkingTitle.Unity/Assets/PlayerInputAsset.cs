using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(PlayerInputAsset), fileName = nameof(PlayerInputAsset))]
    public class PlayerInputAsset : SerializedScriptableObject
    {
        [TitleGroup("Input System")]
        [OdinSerialize]
        [Required]
        public InputActionAsset InputActionAsset { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionMapNameValues))]
        [Required]
        public string InputActionMapName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        public string InputActionMovementName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        public string InputActionRotationName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        public string InputActionPrimaryAttackName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        public string InputActionBoostName { get; set; }
        
        # region Editor

        IEnumerable<string> InputActionMapNameValues => 
            InputActionAsset ? InputActionAsset.actionMaps.Select(e => e.name) : null;

        IEnumerable<string> InputActionNameValues => 
            InputActionAsset ? InputActionAsset.FindActionMap(InputActionMapName)?.actions.Select(e => e.name) : null;

        # endregion
    }
}