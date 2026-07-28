public enum ClapRoundPhase
{
    Ready,
    Playing,
    Finished
}

public readonly struct ClapRoundResult
{
    public int StageNumber { get; }
    public int RawClapCount { get; }
    public bool IsGameOver { get; }
    public int Score => IsGameOver ? 0 : RawClapCount;

    public ClapRoundResult(int stageNumber, int rawClapCount, bool isGameOver)
    {
        StageNumber = stageNumber;
        RawClapCount = rawClapCount;
        IsGameOver = isGameOver;
    }
}
