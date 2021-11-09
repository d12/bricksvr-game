public class LoadingError
{
    private static string _error;
    public static bool IntentionalDisconnect;

    public static void SetError(string error)
    {
        _error = error;
    }

    public static string GetError()
    {
        return _error;
    }

    public static void ClearError()
    {
        _error = null;
    }
}
