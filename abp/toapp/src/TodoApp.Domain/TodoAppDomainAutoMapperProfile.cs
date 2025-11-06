using AutoMapper;

namespace TodoApp;

public class TodoAppDomainAutoMapperProfile : Profile
{
    public TodoAppDomainAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<TodoItem, TodoItemEntityEto>();
    }
}