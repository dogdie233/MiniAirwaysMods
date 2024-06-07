namespace TimeScaler;

public static class Helper
{
    public static float CalculateTimeScale(float level)
        => (Constants.FastSpeedTimeScale - Constants.NormalSpeedTimeScale) * (level - 1) + Constants.NormalSpeedTimeScale;
}
