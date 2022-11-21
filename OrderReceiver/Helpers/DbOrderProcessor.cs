using Microsoft.EntityFrameworkCore;
using OrderReceiver.Models;
using OrderReceiver.Testing;

namespace OrderReceiver.Helpers
{
    //Записывает информацию о заказе в БД
    public sealed class DbOrderProcessor : IOrderProcessor
    {
        private readonly TestDbContext _context;

        public DbOrderProcessor(TestDbContext dbContext)
        {
            _context = dbContext;
        }

        public async ValueTask<Guid> SendToProcess(Order order)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return order.ID;
        }
    }
}
