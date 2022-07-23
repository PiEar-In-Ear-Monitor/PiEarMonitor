namespace PiEarManager.Interfaces
{
    public interface IMulticastLock
    {
        void Acquire();
        bool IsHeld { get; }
        void Release();
    }
}