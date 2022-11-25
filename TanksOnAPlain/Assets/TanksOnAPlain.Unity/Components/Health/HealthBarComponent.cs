using Sirenix.OdinInspector;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Health
{
    public class HealthBarComponent : SerializedMonoBehaviour
    {
        public void OnHealthChanged(object sender, HealthChangedEventArgs e)
        {
            var healthPercentage = e.NewHealth / e.MaxHealth;
            var scale = Mathf.Lerp(0f, 1f, healthPercentage);

            transform.localScale = new Vector3(scale, 1f, 1f);
        }
    }
}