namespace Twittimation
{
    public interface IValue<out T>
    {
        T Get();
    }
}
