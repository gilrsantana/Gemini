using Microsoft.AspNetCore.Identity;

namespace PersonalFinance.Infrastructure.Identity;

public class Role : IdentityRole<Guid>
{
    public string Description { get; private set; } = string.Empty;

    private Role() { }

    private Role(string name, string description)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public static Role Create(string name, string description) => new Role(name, description);
}
