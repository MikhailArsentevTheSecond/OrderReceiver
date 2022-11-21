using OrderReceiver.Models;

namespace OrderReceiver.Helpers
{
    /// <summary>
    /// Передаёт заказ на обработку (или обрабатывает заказ).
    /// </summary>
    public interface IOrderProcessor
    {
        public ValueTask<Guid> SendToProcess(Order order);
    }
}
