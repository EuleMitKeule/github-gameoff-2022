using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using WorkingTitle.Unity.Assets.PowerUps;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.Physics;

namespace WorkingTitle.Unity.Components.UI
{
    public class StatsPanelComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        PowerUpAsset RicochetAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset MovementSpeedAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset ProjectileSpeedAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset MagnetAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset AttackCooldownAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset DamageAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset LifeStealAsset { get; set; }
        
        [OdinSerialize]
        PowerUpAsset MaxHealthAsset { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextKills { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextDamageDone { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextDamageHealed { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextDamageTaken { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextRicochetCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextRicochetValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextMovementSpeedCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextMovementSpeedValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextProjectileSpeedCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextProjectileSpeedValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextMagnetCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextMagnetValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextAttackCooldownCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextAttackCooldownValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextDamageCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextDamageValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextLifeStealCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextLifeStealValue { get; set; }
        
        [OdinSerialize]
        TextMeshProUGUI TextMaxHealthCount { get; set; }
        [OdinSerialize]
        TextMeshProUGUI TextMaxHealthValue { get; set; }
        
        StatisticsComponent StatisticsComponent { get; set; }
        PlayerTankComponent PlayerTankComponent { get; set; }
        AttackComponent PlayerAttackComponent { get; set; }
        TankMovementComponent PlayerMovementComponent { get; set; }
        MagnetComponent PlayerMagnetComponent { get; set; }
        HealthComponent PlayerHealthComponent { get; set; }

        void Awake()
        {
            StatisticsComponent = FindObjectOfType<StatisticsComponent>();
            PlayerTankComponent = FindObjectOfType<PlayerTankComponent>();
            PlayerAttackComponent = PlayerTankComponent.GetComponent<AttackComponent>();
            PlayerMovementComponent = PlayerTankComponent.GetComponent<TankMovementComponent>();
            PlayerMagnetComponent = PlayerTankComponent.GetComponent<MagnetComponent>();
            PlayerHealthComponent = PlayerTankComponent.GetComponent<HealthComponent>();
            
            PlayerHealthComponent.Death += OnPlayerDeath;
            Debug.Log(PlayerHealthComponent);
        }

        void OnPlayerDeath(object sender, EventArgs e)
        {
            Debug.Log("Penis");
            var kills = StatisticsComponent.KillCounts.Values.Sum();
            var damageDone = Mathf.RoundToInt(StatisticsComponent.DamageDone.Values.Sum());
            var damageHealed = StatisticsComponent.HealthRecovered;
            var damageTaken = StatisticsComponent.DamageTaken;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(RicochetAsset))
            {
                var ricochetPowerUpCount = StatisticsComponent.PowerUpCounts[RicochetAsset];
            }
            var ricochetValue = PlayerAttackComponent.Ricochets;
            
            if (StatisticsComponent.PowerUpCounts.ContainsKey(MovementSpeedAsset))
            {
                var movementSpeedPowerUpCount = StatisticsComponent.PowerUpCounts[MovementSpeedAsset];
            }
            var movementSpeedValue = PlayerMovementComponent.MovementSpeed;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(ProjectileSpeedAsset))
            {
                var projectileSpeedPowerUpCount = StatisticsComponent.PowerUpCounts[ProjectileSpeedAsset];
            }
            var projectileSpeedValue = PlayerAttackComponent.ProjectileSpeed;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(MagnetAsset))
            {
                var magnetPowerUpCount = StatisticsComponent.PowerUpCounts[MagnetAsset];
            }
            var magnetValue = PlayerMagnetComponent.Radius;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(AttackCooldownAsset))
            {
                var attackCooldownPowerUpCount = StatisticsComponent.PowerUpCounts[AttackCooldownAsset];
            }
            var attackCooldownValue = PlayerAttackComponent.AttackCooldown;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(DamageAsset))
            {
                var damagePowerUpCount = StatisticsComponent.PowerUpCounts[DamageAsset];
            }
            var damageValue = PlayerAttackComponent.Damage;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(LifeStealAsset))
            {
                var lifeStealPowerUpCount = StatisticsComponent.PowerUpCounts[LifeStealAsset];
            }
            var lifeStealValue = PlayerAttackComponent.LifeSteal;

            if (StatisticsComponent.PowerUpCounts.ContainsKey(MaxHealthAsset))
            {
                var maxHealthPowerUpCount = StatisticsComponent.PowerUpCounts[MaxHealthAsset];
            }
            var maxHealthValue = PlayerHealthComponent.MaxHealth;

            TextKills.SetText($"Kills: {kills}");
            TextDamageDone.text = $"Damage Done: {damageDone}";
            TextDamageHealed.text = $"Damage Healed: {damageHealed}";
            TextDamageTaken.text = $"Damage Taken: {damageTaken}";
            
            TextRicochetCount.text = $"{ricochetPowerUpCount}";
            TextRicochetValue.text = $"{ricochetValue}";
            
            TextMovementSpeedCount.text = $"{movementSpeedPowerUpCount}";
            TextMovementSpeedValue.text = $"{movementSpeedValue}";
            
            TextProjectileSpeedCount.text = $"{projectileSpeedPowerUpCount}";
            TextProjectileSpeedValue.text = $"{projectileSpeedValue}";
            
            TextMagnetCount.text = $"{magnetPowerUpCount}";
            TextMagnetValue.text = $"{magnetValue}";
            
            TextAttackCooldownCount.text = $"{attackCooldownPowerUpCount}";
            TextAttackCooldownValue.text = $"{attackCooldownValue}";
            
            TextDamageCount.text = $"{damagePowerUpCount}";
            TextDamageValue.text = $"{damageValue}";
            
            TextLifeStealCount.text = $"{lifeStealPowerUpCount}";
            TextLifeStealValue.text = $"{lifeStealValue}";
            
            TextMaxHealthCount.text = $"{maxHealthPowerUpCount}";
            TextMaxHealthValue.text = $"{maxHealthValue}";
        }
    }
}