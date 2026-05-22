public class ClapGameModel
{
    public float ElapsedTime { get; set; }
    public int ClapCount { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsSuccess { get; set; }

    public void Reset()
    {
        ElapsedTime = 0f;
        ClapCount = 0;
        IsGameOver = false;
        IsSuccess = false;
    }
}
