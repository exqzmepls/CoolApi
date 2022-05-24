namespace CoolApi.Database.Options
{
    public abstract class BaseReadPortionOptions
    {
        public int Offset { get; init; }

        public int Portion { get; init; }
    }
}
