using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class MessageCenter
{

    private static Dictionary<string, List<Subscription>> subs = new Dictionary<string, List<Subscription>>();

    private static void InnerSend(object sender, string channel, object data)
    {
        if (sender == null)
            throw new NullReferenceException();
        if (string.IsNullOrEmpty(channel))
            throw new NullReferenceException();
        if (subs.TryGetValue(channel, out var subscriptions))
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
    public static void Send(object sender, string channel, object data)
    {
        InnerSend(sender, channel, data);
    }

    public static void Send<TData>(object sender, string channel, TData data)
    {
        InnerSend(sender, channel, data);
    }

    public static void Send<TSender, TData>(TSender sender, string channel, TData data)
    {
        InnerSend(sender, channel, data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="name"></param>
    public static void Send(object sender, string channel)
    {
        InnerSend(sender, channel, null);
    }

    private static void InnerSubscribe(object subscriber, string channel, MethodInfo methodInfo, object target)
    {
        if (subscriber == null)
            throw new NullReferenceException();
        if (string.IsNullOrEmpty(channel))
            throw new NullReferenceException("channel can not be null");
        var sub = new Subscription(subscriber, methodInfo, target);
        if (subs.ContainsKey(channel))
        {
            subs[channel].Add(sub);
            return;
        }
        subs.Add(channel, new List<Subscription>() { sub });
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <typeparam name="TSender">发送者类型</typeparam>
    /// <param name="subscriber">订阅者</param>
    /// <param name="name">管道名</param>
    /// <param name="action">订阅回调</param>
    public static void Subscribe<TSender>(object subscriber, string channel, Action<TSender> action)
    {
        InnerSubscribe(subscriber, channel, action?.Method, action?.Target);
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <typeparam name="TSender">发送者类型</typeparam>
    /// <typeparam name="TData">消息类型</typeparam>
    /// <param name="subscriber">订阅者</param>
    /// <param name="name">管道名</param>
    /// <param name="action">订阅回调</param>
    public static void Subscribe<TSender, TData>(object subscriber, string channel, Action<TSender, TData> action)
    {
        InnerSubscribe(subscriber, channel, action?.Method, action?.Target);
    }

    public static object Subscribe<TSender, TData>(string channel, Action<TSender, TData> action)
    {
        var result = new { };
        InnerSubscribe(result, channel, action?.Method, action?.Target);
        return result;
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="subscriber">订阅者</param>
    /// <param name="name">管道名</param>
    public static void UnSubscribe(object subscriber, string channel)
    {
        if (subs.TryGetValue(channel, out var act))
        {
            if (act == null && act.Count == 0)
            {
                subs.Remove(channel);
                return;
            }
            subs[channel].RemoveAll(x => x.Source == subscriber);
            if (!subs[channel].Any())
                subs.Remove(channel);
            return;
        }
    }
}

