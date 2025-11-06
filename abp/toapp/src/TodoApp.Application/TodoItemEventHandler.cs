using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace TodoApp;

public class TodoItemEventHandler : IDistributedEventHandler<EntityCreatedEto<TodoItemEntityEto>>,
    IDistributedEventHandler<TodoItemDistrEto>,
    ILocalEventHandler<TodoItemLocalEto>,
    ITransientDependency
{
    private readonly ILogger<TodoItemEventHandler> _logger;

    public TodoItemEventHandler(ILogger<TodoItemEventHandler> logger)
    {
        _logger = logger;
    }

    [UnitOfWork]
    public virtual Task HandleEventAsync(EntityCreatedEto<TodoItemEntityEto> eventData)
    {
        _logger.LogInformation("接收到实体ETO事件：{Id} {Text}", eventData.Entity.Id, eventData.Entity.Text);
        return Task.CompletedTask;
    }

    [UnitOfWork]
    public virtual Task HandleEventAsync(TodoItemDistrEto eventData)
    {
        _logger.LogInformation("接收到分布ETO事件：{Text}", eventData.Text);
        return Task.CompletedTask;
    }
    
    [UnitOfWork]
    public Task HandleEventAsync(TodoItemLocalEto eventData)
    {
        _logger.LogInformation("接收到本地ETO事件：{Text}", eventData.Text);
        return Task.CompletedTask;
    }
}