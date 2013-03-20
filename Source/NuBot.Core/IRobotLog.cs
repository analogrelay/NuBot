namespace NuBot
{
    public interface IRobotLog
    {
        void Info(string message, params object[] args);
        void Error(string message, params object[] args);
        void Trace(string message, params object[] args);
    }
}
