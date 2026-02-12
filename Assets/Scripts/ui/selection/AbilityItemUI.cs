using TMPro;
using towers.abilities;
using UnityEngine;
using UnityEngine.UI;

namespace ui.selection
{
    public class AbilityItemUI : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;

        public void Setup(AbilityData ability, int towerLevel)
        {
            if (ability == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_iconImage != null && ability.Icon != null)
            {
                _iconImage.sprite = ability.Icon;
                _iconImage.enabled = true;
            }
            else if (_iconImage != null)
            {
                _iconImage.enabled = false;
            }

            if (_nameText != null)
            {
                _nameText.text = string.IsNullOrEmpty(ability.AbilityName) ? ability.AbilityType.ToString() : ability.AbilityName;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = FormatDescription(ability, towerLevel);
            }
        }

        private string FormatDescription(AbilityData ability, int towerLevel)
        {
            if (string.IsNullOrEmpty(ability.Description))
                return "";

            string desc = ability.Description;
            int level = Mathf.Clamp(towerLevel, 0, 3);

            // Type-specific stat formatting
            switch (ability.AbilityType)
            {
                case AbilityType.SlowOnHit:
                    var slowAbility = ability as SlowOnHitAbility;
                    if (slowAbility != null && slowAbility.SlowPercentByLevel != null && level < slowAbility.SlowPercentByLevel.Length)
                    {
                        desc += $"\n• Slow: {slowAbility.SlowPercentByLevel[level]}%";
                        if (slowAbility.DurationByLevel != null && level < slowAbility.DurationByLevel.Length)
                        {
                            desc += $"\n• Duration: {slowAbility.DurationByLevel[level]:F1}s";
                        }
                    }
                    break;

                case AbilityType.PoisonDOT:
                    var poisonAbility = ability as PoisonDOTAbility;
                    if (poisonAbility != null && poisonAbility.DamagePerTickByLevel != null && level < poisonAbility.DamagePerTickByLevel.Length)
                    {
                        desc += $"\n• Damage: {poisonAbility.DamagePerTickByLevel[level]}/tick";
                        if (poisonAbility.TickIntervalByLevel != null && level < poisonAbility.TickIntervalByLevel.Length)
                        {
                            desc += $" (every {poisonAbility.TickIntervalByLevel[level]:F1}s)";
                        }
                        if (poisonAbility.DurationByLevel != null && level < poisonAbility.DurationByLevel.Length)
                        {
                            desc += $"\n• Duration: {poisonAbility.DurationByLevel[level]:F1}s";
                        }
                    }
                    break;

                case AbilityType.ArmorReduction:
                    var armorAbility = ability as ArmorReductionAbility;
                    if (armorAbility != null && armorAbility.ArmorReductionByLevel != null && level < armorAbility.ArmorReductionByLevel.Length)
                    {
                        desc += $"\n• Armor Reduction: -{armorAbility.ArmorReductionByLevel[level]}";
                        if (armorAbility.DurationByLevel != null && level < armorAbility.DurationByLevel.Length)
                        {
                            desc += $"\n• Duration: {armorAbility.DurationByLevel[level]:F1}s";
                        }
                    }
                    break;

                case AbilityType.BonusDamage:
                    var bonusDamageAbility = ability as BonusDamageAbility;
                    if (bonusDamageAbility != null && bonusDamageAbility.BonusDamageByLevel != null && level < bonusDamageAbility.BonusDamageByLevel.Length)
                    {
                        desc += $"\n• Bonus Damage: +{bonusDamageAbility.BonusDamageByLevel[level]}";
                    }
                    break;

                case AbilityType.AttackSpeedAura:
                    var auraAbility = ability as AttackSpeedAuraAbility;
                    if (auraAbility != null && auraAbility.AttackSpeedMultiplierByLevel != null && level < auraAbility.AttackSpeedMultiplierByLevel.Length)
                    {
                        desc += $"\n• Speed Bonus: +{auraAbility.AttackSpeedMultiplierByLevel[level] * 100:F0}%";
                        if (auraAbility.AuraRadiusByLevel != null && level < auraAbility.AuraRadiusByLevel.Length)
                        {
                            desc += $"\n• Range: {auraAbility.AuraRadiusByLevel[level]:F1} units";
                        }
                    }
                    break;

                case AbilityType.SplashDamage:
                    var splashAbility = ability as SplashDamageAbility;
                    if (splashAbility != null && splashAbility.SplashDamagePercentByLevel != null && level < splashAbility.SplashDamagePercentByLevel.Length)
                    {
                        desc += $"\n• Splash Damage: {splashAbility.SplashDamagePercentByLevel[level]:F0}%";
                        if (splashAbility.SplashRadiusByLevel != null && level < splashAbility.SplashRadiusByLevel.Length)
                        {
                            desc += $"\n• Splash Radius: {splashAbility.SplashRadiusByLevel[level]:F1} units";
                        }
                    }
                    break;

                case AbilityType.MultiTarget:
                    var multiTargetAbility = ability as MultiTargetAbility;
                    if (multiTargetAbility != null && multiTargetAbility.AdditionalTargetsByLevel != null && level < multiTargetAbility.AdditionalTargetsByLevel.Length)
                    {
                        int totalTargets = multiTargetAbility.AdditionalTargetsByLevel[level] + 1;
                        desc += $"\n• Targets: {totalTargets} enemies";
                    }
                    break;

                case AbilityType.IncreasedAttackSpeed:
                    var increasedSpeedAbility = ability as IncreasedAttackSpeedAbility;
                    if (increasedSpeedAbility != null && increasedSpeedAbility.AttackSpeedMultiplierByLevel != null && level < increasedSpeedAbility.AttackSpeedMultiplierByLevel.Length)
                    {
                        desc += $"\n• Attack Speed: +{increasedSpeedAbility.AttackSpeedMultiplierByLevel[level] * 100:F0}%";
                    }
                    break;
            }

            return desc;
        }
    }
}
