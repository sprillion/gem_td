using System;
using System.Collections.Generic;
using skills;
using towers;

namespace infrastructure.services.playerSkillService
{
    public interface IPlayerSkillService
    {
        IReadOnlyList<PlayerSkillInstance> EquippedSkills { get; }
        IReadOnlyList<PlayerSkillData> AvailableSkills { get; }
        bool IsTargeting { get; }

        event Action<int> OnSkillActivated;
        event Action<int> OnSkillCooldownUpdated;
        event Action OnSkillsChanged;
        event Action<int, SkillTargetMode> OnTargetingStarted;
        event Action OnTargetingCancelled;

        void SetAvailableSkills(IReadOnlyList<PlayerSkillData> skills);
        void EquipSkill(int slotIndex, PlayerSkillData data, int upgradeLevel);
        bool CanActivateSkill(int slotIndex);
        void ActivateSkill(int slotIndex);
        void CancelTargeting();
        void OnTargetSelected(Tower tower);
        void ConsumeChanceBoost();

        TowerType? GetActiveTypeChanceBoost();
        float GetTypeChanceBonusPercent();
        int? GetActiveLevelChanceBoost();
        float GetLevelChanceBonusPercent();
    }
}
