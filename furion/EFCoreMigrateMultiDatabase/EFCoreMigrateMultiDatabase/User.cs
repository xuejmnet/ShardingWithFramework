using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreMigrateMultiDatabase;
[Table(nameof(User))]
public class User
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}