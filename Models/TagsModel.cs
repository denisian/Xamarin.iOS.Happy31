using SQLite;

namespace Happy31
{
    // Create the Plain Old CLR Object (POCO) class
    [Table("Tags")]
    public class TagsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    }
}
