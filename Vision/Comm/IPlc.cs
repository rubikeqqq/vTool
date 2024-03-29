namespace Vision.Comm
{
    /// <summary>
    /// PLC接口
    /// </summary>
    public interface IPlc
    {
        bool IsOpened { get; }

        bool OpenPLC();

        void ClosePLC();
    }
}
