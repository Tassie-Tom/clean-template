using System.Xml.Linq;
using Api.SharedKernel;

namespace Api.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, Email email, Name name, string firebaseId)
        : base(id)
    {
        Email = email;
        Name = name;
        FirebaseId = firebaseId;
    }

    private User()
    {
    }

    public Email Email { get; private set; }

    public Name Name { get; private set; }

    public string FirebaseId { get; private set; }

    public static User Create(Email email, Name name, string firebaseId)
    {
        var user = new User(Guid.NewGuid(), email, name, firebaseId);

        user.Raise(new UserCreatedDomainEvent(user.Id));

        return user;
    }
}

