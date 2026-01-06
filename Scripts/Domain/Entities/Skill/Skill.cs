namespace Game.Domain
{
    public class Skill : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Cost { get; private set; }
        public string Description { get; private set; }

        public Skill(int id, string name, int cost, string description)
        {
            Id = id;
            Name = name;
            Cost = cost;
            Description = description;
        }
    }
}