namespace IMS.BusinessModel.Entity
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
