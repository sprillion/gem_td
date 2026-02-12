using System.Collections.Generic;
using enemies;
using towers.abilities.effects;

namespace infrastructure.services.effectService
{
    public interface IEffectService
    {
        void ApplyEffect(Enemy enemy, Effect effect);
        void RemoveEffect(Enemy enemy, string effectId);
        void ClearEffects(Enemy enemy);
        List<Effect> GetActiveEffects(Enemy enemy);
    }
}
