using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace TodoApp;

public class TodoItemEntityEto : EntityEto<Guid>
{
    public string Text { get; set; }
}