using Plata.Models.Interfaces;

namespace Plata.Models.Entities
{
    public partial class UserAccount : IEntity
    {
        public void Edit(IEntity editedEntity)
        {
            if (editedEntity is not UserAccount editedUser) throw new ArgumentException("The `editedEntity` argument must be of `AppUser` type");

            Username = editedUser.Username;
            Password = editedUser.Password;
        }
    }
}
