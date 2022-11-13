namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;
public abstract class Entity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Guid Id { get; }
}
