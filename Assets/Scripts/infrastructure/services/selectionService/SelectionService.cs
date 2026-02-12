using System;
using enemies;
using towers;

namespace infrastructure.services.selectionService
{
 public class SelectionService : ISelectionService
    {
        public SelectionType CurrentSelectionType { get; private set; }
        public Tower SelectedTower { get; private set; }
        public Enemy SelectedEnemy { get; private set; }
        public Tower HoveredTower { get; private set; }

        public event Action<Tower> OnTowerSelected;
        public event Action<Enemy> OnEnemySelected;
        public event Action OnSelectionCleared;
        public event Action<Tower> OnTowerHoverStart;
        public event Action OnTowerHoverEnd;

        public void SelectTower(Tower tower)
        {
            if (tower == null) return;

            // Clear previous selection
            if (CurrentSelectionType == SelectionType.Enemy && SelectedEnemy != null)
            {
                SelectedEnemy.OnDeath -= HandleEnemyDeath;
            }

            SelectedTower = tower;
            SelectedEnemy = null;
            CurrentSelectionType = SelectionType.Tower;

            OnTowerSelected?.Invoke(tower);
        }

        public void SelectEnemy(Enemy enemy)
        {
            if (enemy == null) return;

            // Clear previous selection
            if (CurrentSelectionType == SelectionType.Enemy && SelectedEnemy != null)
            {
                SelectedEnemy.OnDeath -= HandleEnemyDeath;
            }

            SelectedEnemy = enemy;
            SelectedTower = null;
            CurrentSelectionType = SelectionType.Enemy;

            // Auto-clear selection when enemy dies
            enemy.OnDeath += HandleEnemyDeath;

            OnEnemySelected?.Invoke(enemy);
        }

        public void ClearSelection()
        {
            if (CurrentSelectionType == SelectionType.Enemy && SelectedEnemy != null)
            {
                SelectedEnemy.OnDeath -= HandleEnemyDeath;
            }

            SelectedTower = null;
            SelectedEnemy = null;
            CurrentSelectionType = SelectionType.None;

            OnSelectionCleared?.Invoke();
        }

        public void SetHoveredTower(Tower tower)
        {
            if (HoveredTower == tower) return;

            if (HoveredTower != null)
            {
                OnTowerHoverEnd?.Invoke();
            }

            HoveredTower = tower;

            if (HoveredTower != null)
            {
                OnTowerHoverStart?.Invoke(tower);
            }
        }

        public void ClearHover()
        {
            if (HoveredTower != null)
            {
                OnTowerHoverEnd?.Invoke();
                HoveredTower = null;
            }
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            if (SelectedEnemy == enemy)
            {
                ClearSelection();
            }
        }
    }
}
