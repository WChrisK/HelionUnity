namespace Helion.Core.Resource.Decorate.Definitions.Types
{
    /// <summary>
    /// A set of flags that allow quick looking up of certain types that are
    /// really common and do not warrant extra computation time to resolve if
    /// they are of that type or not.
    /// </summary>
    public enum ActorType
    {
        Ammo,
        Inventory,
        Player,
        Powerup,
        Projectile,
        SpawnPoint,
        TeleportDestination,
        Weapon
    }
}
