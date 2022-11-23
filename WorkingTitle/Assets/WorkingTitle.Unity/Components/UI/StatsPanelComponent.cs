using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Assets.PowerUps;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.Physics;
using WorkingTitle.Unity.Components.PowerUps;

namespace WorkingTitle.Unity.Components.UI
{
    public class StatsPanelComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] 
        Dictionary<SkillType, GameObject> SkillToPanel { get; set; } = new();
        
        [OdinSerialize]
        TextMeshProUGUI TextKills { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextDamageDone { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextDamageHealed { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextDamageTaken { get; set; }
        
        GameComponent GameComponent { get; set; }
        StatisticsComponent StatisticsComponent { get; set; }
        PlayerTankComponent PlayerTankComponent { get; set; }
        AttackComponent PlayerAttackComponent { get; set; }
        TankMovementComponent PlayerMovementComponent { get; set; }
        MagnetComponent PlayerMagnetComponent { get; set; }
        HealthComponent PlayerHealthComponent { get; set; }
        PowerUpConsumerComponent PlayerPowerUpConsumerComponent { get; set; }
        CanvasGroup CanvasGroup { get; set; }
        
        void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            GameComponent = FindObjectOfType<GameComponent>();
            StatisticsComponent = FindObjectOfType<StatisticsComponent>();
            PlayerTankComponent = FindObjectOfType<PlayerTankComponent>();
            PlayerAttackComponent = PlayerTankComponent.GetComponent<AttackComponent>();
            PlayerMovementComponent = PlayerTankComponent.GetComponent<TankMovementComponent>();
            PlayerMagnetComponent = PlayerTankComponent.GetComponent<MagnetComponent>();
            PlayerHealthComponent = PlayerTankComponent.GetComponent<HealthComponent>();
            PlayerPowerUpConsumerComponent = PlayerTankComponent.GetComponent<PowerUpConsumerComponent>();
            
            PlayerHealthComponent.Death += OnPlayerDeath;
        }

        [UsedImplicitly]
        public void OnContinueClicked()
        {
            GameComponent.ReloadScene();
        }
        
        void OnPlayerDeath(object sender, EventArgs e)
        {
            CanvasGroup.alpha = 1;
            
            var kills = StatisticsComponent.KillCounts.Values.Sum();
            var damageDone = Mathf.RoundToInt(StatisticsComponent.DamageDone.Values.Sum());
            var damageHealed = StatisticsComponent.HealthRecovered;
            var damageTaken = StatisticsComponent.DamageTaken;

            foreach (var (skillType, panelObject) in SkillToPanel)
            {
                var powerUpAsset = GameComponent.GameAsset.TankAssets[skillType].PowerUpAsset;
                
                var powerUpCount = PlayerPowerUpConsumerComponent.PowerUpsConsumed.ContainsKey(powerUpAsset) ?
                    PlayerPowerUpConsumerComponent.PowerUpsConsumed[powerUpAsset] :
                    0;
                var powerUpValue = skillType switch
                {
                    SkillType.Ricochet => PlayerAttackComponent.Ricochets,
                    SkillType.MovementSpeed => PlayerMovementComponent.MovementSpeed,
                    SkillType.ProjectileSpeed => PlayerAttackComponent.ProjectileSpeed,
                    SkillType.Magnet => PlayerMagnetComponent.Radius,
                    SkillType.AttackCooldown => PlayerAttackComponent.AttackCooldown,
                    SkillType.Damage => PlayerAttackComponent.Damage,
                    SkillType.LifeSteal => PlayerAttackComponent.LifeSteal,
                    SkillType.MaxHealth => PlayerHealthComponent.MaxHealth,
                    _ => throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null)
                };

                var countText = panelObject.transform.GetChild(1);
                var valueText = panelObject.transform.GetChild(2);
                
                countText.GetComponent<TextMeshProUGUI>().text = $"{powerUpCount}";
                valueText.GetComponent<TextMeshProUGUI>().text = $"{powerUpValue}";
            }

            TextKills.SetText($"Kills: {kills}");
            TextDamageDone.text = $"Damage Done: {damageDone}";
            TextDamageHealed.text = $"Damage Healed: {damageHealed}";
            TextDamageTaken.text = $"Damage Taken: {damageTaken}";
        }
    }
}