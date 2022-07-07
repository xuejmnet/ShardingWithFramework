using System;
using Volo.Abp.Domain.Entities;

namespace TodoApp
{
    //不做时间分片所以不需要提前赋值
    public class TodoItem : BasicAggregateRoot<Guid>,IShardingKeyIsGuId//,IShardingKeyIsCreationTime
    {
        public string Text { get; set; }
    }
}