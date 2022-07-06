using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoApp;

public class TodoItem:IEntity, IEntityTypeBuilder<TodoItem>
{
    public string Id { get; set; }
    public string Text { get; set; }
    public void Configure(EntityTypeBuilder<TodoItem> entityBuilder, DbContext dbContext, Type dbContextLocator)
    {
        entityBuilder.HasKey(o => o.Id);
        entityBuilder.Property(o => o.Id).IsRequired().HasMaxLength(50).HasComment("id");
        entityBuilder.Property(o => o.Text).IsRequired().HasMaxLength(256).HasComment("事情");
        entityBuilder.ToTable(nameof(TodoItem));
    }
}