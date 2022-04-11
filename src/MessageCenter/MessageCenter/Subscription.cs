using System;
using System.Reflection;


internal class Subscription
{
    public Guid Id { get; set; }
    public object Source { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public object Target { get; set; }
    public Subscription(object source, MethodInfo methodInfo, object target)
    {
        Id = Guid.NewGuid();
        Source = source ?? throw new NullReferenceException("source can not be null");
        MethodInfo = methodInfo ?? throw new NullReferenceException("callback can not be null");
        Target = target;
    }


    public void Invoke(object sender, object data)
    {
        //只需要判断sender为空的情况，sender==Source允许
        if (sender == null)
            return;
        if (MethodInfo.IsStatic)
        {
            MethodInfo.Invoke(null, MethodInfo.GetParameters().Length == 1 ? new[] { sender } : new[] { sender, data });
            return;
        }
        MethodInfo?.Invoke(Target, MethodInfo.GetParameters().Length == 2 ? new[] { sender, data } : new[] { sender });
    }
}