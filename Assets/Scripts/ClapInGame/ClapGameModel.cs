public class ClapGameModel
{
    public ClapRoundPhase Phase { get; private set; } = ClapRoundPhase.Ready;
    public float ElapsedTime { get; private set; }
    public int ClapCount { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score => IsGameOver ? 0 : ClapCount;

    public void StartRound()
    {
        Phase = ClapRoundPhase.Playing;
        ElapsedTime = 0f;
        ClapCount = 0;
        IsGameOver = false;
    }

    public void UpdateElapsedTime(float elapsedTime)
    {
        ElapsedTime = elapsedTime;
    }

    public void RegisterClap(bool isOvertime)
    {
        if (Phase != ClapRoundPhase.Playing)
        {
            return;
        }

        ClapCount++;
        IsGameOver |= isOvertime;
    }

    public void FinishRound()
    {
        if (Phase == ClapRoundPhase.Playing)
        {
            Phase = ClapRoundPhase.Finished;
        }
    }
}
