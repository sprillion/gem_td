using System;
using towers;
using Zenject;

namespace infrastructure.services.selectionService
{
    public class RangeCircleManager : IInitializable, IDisposable
    {
        private readonly ISelectionService _selectionService;
        private Tower _currentlyShownTower;

        [Inject]
        public RangeCircleManager(ISelectionService selectionService)
        {
            _selectionService = selectionService;
        }

        public void Initialize()
        {
            _selectionService.OnTowerSelected += HandleTowerSelected;
            _selectionService.OnSelectionCleared += HandleSelectionCleared;
            _selectionService.OnEnemySelected += _ => HandleSelectionCleared();
        }

        public void Dispose()
        {
            _selectionService.OnTowerSelected -= HandleTowerSelected;
            _selectionService.OnSelectionCleared -= HandleSelectionCleared;
        }

        private void HandleTowerSelected(Tower tower)
        {
            // Hide circle from previously selected tower
            if (_currentlyShownTower != null && _currentlyShownTower != tower)
            {
                HideRangeCircle(_currentlyShownTower);
            }

            ShowRangeCircle(tower);
            _currentlyShownTower = tower;
        }

        private void HandleSelectionCleared()
        {
            if (_currentlyShownTower != null)
            {
                HideRangeCircle(_currentlyShownTower);
                _currentlyShownTower = null;
            }
        }

        private void ShowRangeCircle(Tower tower)
        {
            if (tower == null) return;

            var rangeDrawer = tower.GetComponent<RangeCircleDrawer>();
            if (rangeDrawer != null)
            {
                rangeDrawer.Show();
            }
        }

        private void HideRangeCircle(Tower tower)
        {
            if (tower == null) return;

            var rangeDrawer = tower.GetComponent<RangeCircleDrawer>();
            if (rangeDrawer != null)
            {
                rangeDrawer.Hide();
            }
        }
    }
}
