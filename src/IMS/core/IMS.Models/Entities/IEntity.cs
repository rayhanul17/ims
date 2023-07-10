namespace IMS.Models.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
