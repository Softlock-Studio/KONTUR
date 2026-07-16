namespace Game.House
{
    public interface IResourceProvider
    {
        bool TrySpend(ResourceType type, int amount);
    }
}
