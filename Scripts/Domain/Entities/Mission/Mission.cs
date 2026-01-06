namespace Game.Domain
{
    public class Mission : IEntity
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }

        public Mission(int id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
            IsCompleted = false;
        }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
}