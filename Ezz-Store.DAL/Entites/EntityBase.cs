namespace Ezz_Store.DAL.Entites;

public abstract class EntityBase
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
