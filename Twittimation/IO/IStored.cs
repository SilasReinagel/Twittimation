using System;

namespace Twittimation.IO
{
    public interface IStored<T>
    {
        T Get();
        void Update(Func<T, T> update);
    }
}
