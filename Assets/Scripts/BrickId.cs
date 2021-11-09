public static class BrickId
{
    public static string FetchNewBrickID()
    {
        return System.Guid.NewGuid().ToString();
    }
}