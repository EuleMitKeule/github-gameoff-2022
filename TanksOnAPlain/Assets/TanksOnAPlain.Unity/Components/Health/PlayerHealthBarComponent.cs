using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Health
{
    public class PlayerHealthBarComponent : SerializedMonoBehaviour
    {
        float Health { get; set; }
        
        float MaxHealth { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI Text { get; set; }
        RectTransform RectTransform { get; set; }
        GameComponent GameComponent { get; set; }
        HealthComponent PlayerHealthComponent { get; set; }

        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            GameComponent = GetComponentInParent<GameComponent>();
            
            PlayerHealthComponent = GameComponent.PlayerObject.GetComponent<HealthComponent>();
            PlayerHealthComponent.HealthChanged += OnPlayerHealthChanged;
            PlayerHealthComponent.InvokeHealthChanged();
        }

        void OnPlayerHealthChanged(object sender, HealthChangedEventArgs e)
        {
            var ratio = e.NewHealth / e.MaxHealth;
            var text = $"{(int)e.NewHealth}/{(int)e.MaxHealth}";
            
            RectTransform.localScale = new Vector3(ratio, 1, 1);
            Text.text = text;
        }
    }
}