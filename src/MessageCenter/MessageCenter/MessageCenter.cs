using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class MessageCenter
{
    class Subscription
    {
        public Guid Id { get; set; }
        public object Source { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public object Target { get; set; }
        public Subscription(object source, MethodInfo methodInfo, object target)
        {
            Id = Guid.NewGuid();
            Source = source;
            MethodInfo = methodInfo;
            Target = target;
        }
        public void Invoke(object sender, object data)
        {
            try
            {
                if (sender == null || sender == Source) return;
                if (MethodInfo.IsStatic)
                {
                    MethodInfo.Invoke(null, MethodInfo.GetParameters().Length == 1 ? new[] { sender } : new[] { sender, data });
                    return;
                }
                MethodInfo?.Invoke(Target, MethodInfo.GetParameters().Length == 2 ? new[] { sender, data } : new[] { sender });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    private static Dictionary<string, List<Subscription>> subs = new Dictionary<string, List<Subscription>>();

    public static void InnerSend(object sender, string name, object data)
    {
        if (string.IsNullOrEmpty(name))
            throw new NullReferenceException();
        if (subs.TryGetValue(name, out var subscriptions))
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Invoke(sender, data);
            }
        }
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="name">管道名</param>
    /// <param name="data">消息</param>
    public static void Send(object sender, string name, object data)
    {
        InnerSend(sender, name, data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="name"></param>
    public static void Send(object sender, string name)
    {
        InnerSend(sender, name, null);
    }

    private static void InnerSubscribe(object subscriber, string name, MethodInfo methodInfo, object target)
    {
        if (subscriber == null)
            throw new NullReferenceException();
        if (string.IsNullOrEmpty(name))
            throw new NullReferenceException();
        var sub = new Subscription(subscriber, methodInfo, target);
        if (subs.ContainsKey(name))
        {
            subs[name].Add(sub);
            return;
        }
        subs.Add(name, new List<Subscription>() { sub });
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <typeparam name="TSender">发送者类型</typeparam>
    /// <param name="subscriber">订阅者</param>
    /// <param name="name">管道名</param>
    /// <param name="action">订阅回调</param>
    public static void Subscribe<TSender>(object subscriber, string name, Action<TSender> action)
    {
        InnerSubscribe(subscriber, name, action?.Method, action?.Target);
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <typeparam name="TSender">发送者类型</typeparam>
    /// <typeparam name="TData">消息类型</typeparam>
    /// <param name="subscriber">订阅者</param>
    /// <param name="name">管道名</param>
    /// <param name="action">订阅回调</param>
    public static void Subscribe<TSender, TData>(object subscriber, string name, Action<TSender, TData> action)
    {
        InnerSubscribe(subscriber, name, action?.Method, action?.Target);
    }

    public static object Subscribe<TSender, TData>(string name, Action<TSender, TData> action)
    {
        var result = new { };
        InnerSubscribe(result, name, action?.Method, action?.Target);
        return result;
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="subscriber">订阅者</param>
    /// <param name="name">管道名</param>
    public static void UnSubscribe(object subscriber, string name)
    {
        if (subs.TryGetValue(name, out var act))
        {
            if (act == null && act.Count == 0)
            {
                subs.Remove(name);
                return;
            }
            subs[name].RemoveAll(x => x.Source == subscriber);
            if (!subs[name].Any())
                subs.Remove(name);
            return;
        }
    }
}


