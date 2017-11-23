using SQLite;

namespace Happy31
{
    // Create the Plain Old CLR Object (POCO) class
    [Table("permissions")]
    public class PermissionsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Role { get; set; }
    }
}
