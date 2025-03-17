using System;
using System.Threading;
using System.Threading.Tasks;


namespace MsgApp.Services
{
  public class ProductionTimer : ITimerService
  {
    public async Task DelayAsync(TimeSpan delay, CancellationToken token)
    {
        await Task.Delay(delay, token);
    }
  }
}