# MessageCenter

简单的C#进程内消息转发扩展

## 使用

```C#
Foo foo = new Foo();
Bar bar = new Bar();
//注意，要先注册消息订阅
bar.Start();
foo.Start();



public class Bar
{
    public void Start()
    {
        MessageCenter.Subscribe<Foo, string>(this, "测试通道", Handle);
    }

    private void Handle(Foo sender, string data)
    {
        Console.WriteLine(data);
    }
}

public class Foo
{
    public void Start()
    {
        MessageCenter.Send(this, "测试通道", "这是一条测试信息");
    }
}
```