namespace Game.Save
{
    public interface ISaveService
    {
        void Save(SaveData data);
        bool TryLoad(out SaveData data);
    }
}
