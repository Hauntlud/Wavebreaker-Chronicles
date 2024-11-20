[System.Serializable]
public class XPSaveData
{
    public int currentLevel;
    public int currentXP;
    public int xpToNextLevel;

    public XPSaveData(int currentLevel, int currentXP, int xpToNextLevel)
    {
        this.currentLevel = currentLevel;
        this.currentXP = currentXP;
        this.xpToNextLevel = xpToNextLevel;
    }
}