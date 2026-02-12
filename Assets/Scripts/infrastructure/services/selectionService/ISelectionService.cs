using System;
using enemies;
using towers;

namespace infrastructure.services.selectionService
{
    public enum SelectionType { None, Tower, Enemy }

    public interface ISelectionService
    {
        SelectionType CurrentSelectionType { get; }
        Tower SelectedTower { get; }
        Enemy SelectedEnemy { get; }
        Tower HoveredTower { get; }

        void SelectTower(Tower tower);
        void SelectEnemy(Enemy enemy);
        void ClearSelection();
        void SetHoveredTower(Tower tower);
        void ClearHover();

        event Action<Tower> OnTowerSelected;
        event Action<Enemy> OnEnemySelected;
        event Action OnSelectionCleared;
        event Action<Tower> OnTowerHoverStart;
        event Action OnTowerHoverEnd;
    }
}
