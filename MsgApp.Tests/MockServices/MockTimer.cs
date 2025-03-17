using MsgApp.Services;


public class MockTimer : ITimerService
{
    //  Zähler um zu sehen, ob DelayAsync aufgerufen wurde.
    public int CallCount { get; private set; }

    public Task DelayAsync(TimeSpan delay, CancellationToken token)
    {
        CallCount++;

        // sofort abgeschlossenes Task statt Task.Delay(delay, token)
        return Task.CompletedTask;
    }
}