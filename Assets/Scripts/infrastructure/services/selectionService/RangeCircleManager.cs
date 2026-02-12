using System;
using towers;
using Zenject;

namespace infrastructure.services.selectionService
{
    public class RangeCircleManager : IInitializable, IDisposable
    {
        private readonly ISelectionService _selectionService;

        [Inject]
        public RangeCircleManager(ISelectionService selectionService)
        {
            _selectionService = selectionService;
        }

        public void Initialize()
        {
            _selectionService.OnTowerHoverStart += HandleTowerHoverStart;
            _selectionService.OnTowerHoverEnd += HandleTowerHoverEnd;
            _selectionService.OnTowerSelected += HandleTowerSelected;
            _selectionService.OnSelectionCleared += HandleSelectionCleared;
        }

        public void Dispose()
        {
            _selectionService.OnTowerHoverStart -= HandleTowerHoverStart;
            _selectionService.OnTowerHoverEnd -= HandleTowerHoverEnd;
            _selectionService.OnTowerSelected -= HandleTowerSelected;
            _selectionService.OnSelectionCleared -= HandleSelectionCleared;
        }

        private void HandleTowerHoverStart(Tower tower)
        {
            // Only show hover circle if this tower is not already selected
            if (_selectionService.SelectedTower != tower)
            {
                ShowRangeCircle(tower);
            }
        }

        private void HandleTowerHoverEnd()
        {
            // Only hide hover circle if no tower is selected
            if (_selectionService.CurrentSelectionType != SelectionType.Tower)
            {
                HideRangeCircle(_selectionService.HoveredTower);
            }
        }

        private void HandleTowerSelected(Tower tower)
        {
            // Hide circle from previously selected tower
            if (_selectionService.SelectedTower != null && _selectionService.SelectedTower != tower)
            {
                HideRangeCircle(_selectionService.SelectedTower);
            }

            // Show circle for newly selected tower
            ShowRangeCircle(tower);
        }

        private void HandleSelectionCleared()
        {
            // Hide circle if a tower was previously selected
            if (_selectionService.SelectedTower != null)
            {
                HideRangeCircle(_selectionService.SelectedTower);
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
