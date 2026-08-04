using System.Collections.Generic;
using Game.AI.Employee;
using Game.House;
using Game.House.Model;
using Loader.SceneController;
using VContainer.Unity;

namespace Game.Save
{
    // Writes the autosave checkpoint once, at the start of each level, so a fresh save
    // always reflects what the player is walking into (resource counts, surviving
    // headcount). Kept separate from MissionManager, which owns win/lose/timer, not
    // persistence.
    public sealed class LevelStartSaveTrigger : IStartable
    {
        private readonly ISaveService saveService;
        private readonly ResourceInventory resourceInventory;
        private readonly EmployeeRegistry employeeRegistry;
        private readonly SceneController sceneController;

        public LevelStartSaveTrigger(ISaveService saveService, ResourceInventory resourceInventory,
            EmployeeRegistry employeeRegistry, SceneController sceneController)
        {
            this.saveService = saveService;
            this.resourceInventory = resourceInventory;
            this.employeeRegistry = employeeRegistry;
            this.sceneController = sceneController;
        }

        public void Start()
        {
            var data = new SaveData
            {
                LevelType = sceneController.GetCurrentLevelType(),
                AliveEmployeeCount = CountAliveEmployees(),
            };

            foreach (KeyValuePair<ResourceType, int> pair in resourceInventory.GetAllCounts())
                data.ResourceCounts.Add(new ResourceCountEntry { Type = pair.Key, Count = pair.Value });

            saveService.Save(data);
        }

        private int CountAliveEmployees()
        {
            int count = 0;
            foreach (IEmployee employee in employeeRegistry.Employees)
                if (employee.IsAlive) count++;

            return count;
        }
    }
}
