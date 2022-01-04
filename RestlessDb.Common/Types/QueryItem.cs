namespace RestlessDb.Common.Types
{
    public class QueryItem
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Sql { get; set; }
        public string Parent { get; set; }
        public int Pos { get; set; }
    }
}
