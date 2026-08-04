using System.IO;
using UnityEngine;

namespace Game.Save
{
    // Single-slot autosave: every call to Save overwrites the same file. There is no
    // multi-slot support and nothing currently calls TryLoad — see LevelStartSaveTrigger
    // for the write side; the read side is a deliberate stub for the future "continue game"
    // flow (Docs/agents/map.md: between-levels screen is still Planned).
    public sealed class SaveService : ISaveService
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");

        public void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(SavePath, json);
        }

        public bool TryLoad(out SaveData data)
        {
            if (!File.Exists(SavePath))
            {
                data = null;
                return false;
            }

            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<SaveData>(json);
            return data != null;
        }
    }
}
