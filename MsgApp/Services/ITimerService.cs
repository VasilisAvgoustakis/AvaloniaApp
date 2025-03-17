using System;
using System.Threading;
using System.Threading.Tasks;

namespace MsgApp.Services
{
  public interface ITimerService
  {
    Task DelayAsync(TimeSpan delay, CancellationToken Token);
  }
}