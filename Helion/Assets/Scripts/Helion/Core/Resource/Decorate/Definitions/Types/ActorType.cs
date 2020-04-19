namespace Helion.Core.Resource.Decorate.Definitions.Types
{
    /// <summary>
    /// A set of flags that allow quick looking up of certain types that are
    /// really common and do not warrant extra computation time to resolve if
    /// they are of that type or not.
    /// </summary>
    /// <remarks>
    /// If a new field is added, it should also be added in the ActorTypes
    /// object with a new property getting the value.
    /// </remarks>
    public enum ActorType
    {
        Ammo,
        Inventory,
        Key,
        Player,
        Powerup,
        SpawnPoint,
        TeleportDestination,
        Weapon
    }
}
