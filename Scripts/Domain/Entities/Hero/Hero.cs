namespace Game.Domain
{
    public class Hero : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        
        public Hero(int id, string name)
        {
            Id = id;
            Name = name;
            Level = 1;
            Experience = 0;
        }
    }
}