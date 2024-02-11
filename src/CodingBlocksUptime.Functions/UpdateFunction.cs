using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CodingBlocksUptime.Functions;

public class UpdateFunction(ILoggerFactory loggerFactory)
{
    private readonly ILogger Logger = loggerFactory.CreateLogger<UpdateFunction>();

    [Function("UpdateFunction")]
    public void Run(
#if DEBUG
    [TimerTrigger("0 0 0 1 1 0", RunOnStartup = true)] TimerInfo myTimer
#else
    [TimerTrigger("0 0 * * * *")] TimerInfo myTimer
#endif
        )
    {

    }
}
