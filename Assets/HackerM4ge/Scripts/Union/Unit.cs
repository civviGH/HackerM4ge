

public sealed class Unit
{

    private static Unit instance;

    private Unit() { }

    public static Unit Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Unit();
            }
            return instance;
        }
    }
}
