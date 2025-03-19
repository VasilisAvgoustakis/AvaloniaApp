using MsgApp.Services;


public class MockTimer : ITimerService
{
    //  Zähler um zu sehen, ob DelayAsync aufgerufen wurde.
    public int CallCount { get; private set; }=0;

    public Task DelayAsync(TimeSpan delay, CancellationToken token)
    {
        CallCount++;

        // sofort abgeschlossenes Task statt Task.Delay(delay, token)
        return Task.CompletedTask;
    }
}