using MsgApp.Models;
using MsgApp.ViewModels;

[TestFixture]
public class MessageViewModelTests
{
  [Test]
  public void MessageViewModel_ShouldReflectModelChanges()
  {
    var message = new Message { SenderName = "Alice", IsRead = false };
    var vm = new MessageViewModel(message);

    Assert.That(vm.SenderName, Is.EqualTo("Alice"));
    Assert.That(vm.IsRead, Is.False);

    vm.IsRead = true;
    Assert.That(message.IsRead, Is.True);
  }

}
