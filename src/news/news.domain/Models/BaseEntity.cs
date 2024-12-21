namespace news.domain.Models
{
    public abstract class BaseEntity
    {


        public Guid Id { get; protected set; }

        protected BaseEntity(Guid id)
        {
            Id = id;
        }
        protected BaseEntity()
        {

        }

        public override bool Equals(object? obj)
        {
            return obj is BaseEntity entity &&
                   Id.Equals(entity.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
