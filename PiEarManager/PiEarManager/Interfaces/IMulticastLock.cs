namespace PiEar.Interfaces
{
    public interface IMulticastLock
    {
        void Acquire();
        bool IsHeld { get; }
        void Release();
    }
}