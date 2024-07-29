namespace Plata.Models.Interfaces
{
    public interface IEntity
    {
        public int Id { get; set; }
        public void Edit(IEntity editedEntity);
        bool Equals(object obj);
    }
}
