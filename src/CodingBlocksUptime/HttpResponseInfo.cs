namespace CodingBlocksUptime;

public class HttpResponseInfo(double timeMsec, int code, string content)
{
    public readonly DateTime DateTime = DateTime.UtcNow.ToUniversalTime();
    public readonly int Code = code;
    public readonly double TimeMsec = timeMsec;
    public readonly string Content = content;
    public int Size => Content.Length;
    public DatabaseRecord DatabaseRecord => new(DateTime, Code, Size, TimeMsec);

    public override string ToString()
    {
        string code = Code switch
        {
            200 => "success",
            404 => "not found",
            524 => "timeout",
            _ => Code.ToString(),
        };

        return $"Response '{code}' ({Size} bytes) after {TimeMsec} ms";
    }
}