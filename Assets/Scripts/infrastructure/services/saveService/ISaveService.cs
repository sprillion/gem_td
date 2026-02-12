using System;

namespace infrastructure.services.saveService
{
    public interface ISaveService
    {
        bool HasSaveData { get; }

        event Action<GameSaveData> OnGameSaved;
        event Action<GameSaveData> OnGameLoaded;

        void SaveGame();
        GameSaveData LoadGame();
        void DeleteSave();
        bool TryLoadGame(out GameSaveData saveData);
    }
}
