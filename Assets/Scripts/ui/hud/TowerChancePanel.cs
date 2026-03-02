using infrastructure.services.playerService;
using infrastructure.services.playerSkillService;
using TMPro;
using towers;
using UnityEngine;
using Zenject;

namespace ui.hud
{
    public class TowerChancePanel : MonoBehaviour
    {
        [Header("Tower Type Chances (P,Q,D,G,E,R,B,Y)")]
        [SerializeField] private TextMeshProUGUI[] _towerTypeTexts = new TextMeshProUGUI[8];

        [Header("Tower Level Chances (L0,L1,L2,L3,L4)")]
        [SerializeField] private TextMeshProUGUI[] _towerLevelTexts = new TextMeshProUGUI[5];

        private IPlayerSkillService _playerSkillService;
        private IPlayerService _playerService;

        private static readonly TowerType[] TowerTypes =
        {
            TowerType.P, TowerType.Q, TowerType.D, TowerType.G,
            TowerType.E, TowerType.R, TowerType.B, TowerType.Y
        };

        [Inject]
        private void Construct(IPlayerSkillService playerSkillService, IPlayerService playerService)
        {
            _playerSkillService = playerSkillService;
            _playerService = playerService;
        }

        private void Start()
        {
            SubscribeToEvents();
            Refresh();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _playerSkillService.OnSkillActivated += OnIntEvent;
            _playerSkillService.OnSkillCooldownUpdated += OnIntEvent;
            _playerService.OnLevelChanged += OnIntEvent;
        }

        private void UnsubscribeFromEvents()
        {
            if (_playerSkillService != null)
            {
                _playerSkillService.OnSkillActivated -= OnIntEvent;
                _playerSkillService.OnSkillCooldownUpdated -= OnIntEvent;
            }

            if (_playerService != null)
            {
                _playerService.OnLevelChanged -= OnIntEvent;
            }
        }

        private void OnIntEvent(int _) => Refresh();

        private void Refresh()
        {
            RefreshTypeTexts();
            RefreshLevelTexts();
        }

        private void RefreshTypeTexts()
        {
            var boostType = _playerSkillService.GetActiveTypeChanceBoost();
            float bonus = _playerSkillService.GetTypeChanceBonusPercent() / 100f;
            float baseChance = 1f / 8f;

            for (int i = 0; i < TowerTypes.Length && i < _towerTypeTexts.Length; i++)
            {
                var text = _towerTypeTexts[i];
                if (text == null) continue;

                string typeName = TowerTypes[i].ToString();
                float chance;

                if (boostType.HasValue && boostType.Value == TowerTypes[i])
                {
                    chance = bonus + (1f - bonus) * baseChance;
                    float displayPct = chance * 100f;
                    float bonusPct = _playerSkillService.GetTypeChanceBonusPercent();
                    text.text = $"<color=yellow>{typeName}: {displayPct:F1}% (+{bonusPct:F0}%)</color>";
                }
                else
                {
                    if (boostType.HasValue)
                        chance = (1f - bonus) * baseChance;
                    else
                        chance = baseChance;

                    float displayPct = chance * 100f;
                    text.text = $"{typeName}:   {displayPct:F1}%";
                }
            }
        }

        private void RefreshLevelTexts()
        {
            var boostLevel = _playerSkillService.GetActiveLevelChanceBoost();
            float bonus = _playerSkillService.GetLevelChanceBonusPercent() / 100f;

            // Get base weights from PlayerService (reads from PlayerProgressionData)
            float[] rawWeights = _playerService.GetTowerLevelBaseWeights();

            // Normalize to sum = 1
            float total = 0f;
            foreach (var w in rawWeights) total += w;
            float[] baseWeights = new float[rawWeights.Length];
            if (total > 0f)
                for (int i = 0; i < rawWeights.Length; i++) baseWeights[i] = rawWeights[i] / total;

            int count = Mathf.Min(baseWeights.Length, _towerLevelTexts.Length);
            for (int i = 0; i < count; i++)
            {
                var text = _towerLevelTexts[i];
                if (text == null) continue;

                float chance;

                if (boostLevel.HasValue && boostLevel.Value == i)
                {
                    chance = bonus + (1f - bonus) * baseWeights[i];
                    float displayPct = chance * 100f;
                    float bonusPct = _playerSkillService.GetLevelChanceBonusPercent();
                    text.text = $"<color=yellow>L{i}: {displayPct:F1}% (+{bonusPct:F0}%)</color>";
                }
                else
                {
                    chance = boostLevel.HasValue ? (1f - bonus) * baseWeights[i] : baseWeights[i];
                    float displayPct = chance * 100f;
                    text.text = $"{i + 1}:   {displayPct:F1}%";
                }
            }
        }
    }
}
