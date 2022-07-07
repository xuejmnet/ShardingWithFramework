using System;
using Volo.Abp.Domain.Entities;

namespace TodoApp
{
    public class TodoItem : BasicAggregateRoot<Guid>,IShardingKeyIsGuId,IShardingKeyIsCreationTime
    {
        public string Text { get; set; }
    }
}