using MessagePack;

namespace ConsoleApp1;
[MessagePackObject()]
public class Test
{
    [Key(0)]
    public string Id { get; set; }
    [Key(1)]
    public int Age { get; set; }
}