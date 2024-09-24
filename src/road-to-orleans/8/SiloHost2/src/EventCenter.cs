using Orleans;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SiloHost2;

public class EventCenter
{
    private readonly Dictionary<Type, MethodInfo> _dictionary;
    private readonly Dictionary<Type, BatchWorkerFromDelegate> _works;

    public EventCenter()
    {
        _dictionary = [];
        _works = [];
    }

    #region Methods

    public void Register(EventData eventData, MethodInfo method)
    {
        var type = eventData.GetType();
        _dictionary.Add(type, method);

        _works.Add(
            type,
            new BatchWorkerFromDelegate(
                () =>
                {
                    // get EventData from database
                    // create grain
                    // invoke grain method
                    // update EventData
                    return Task.CompletedTask;
                },
                default));
    }

    public void Signal(EventData eventData)
    {
        if (_works.TryGetValue(eventData.GetType(), out var worker))
        {
            worker.Notify();
        }
    }

    #endregion

}

public class EventBus
{
    private readonly EventCenter _eventCenter;
    private readonly List<EventData> _eventDatas;

    public EventBus(EventCenter eventCenter)
    {
        _eventCenter = eventCenter;
        _eventDatas = [];
    }

    #region Methods

    public void Commit()
    {
        // save EventData to database

        foreach (var eventData in _eventDatas)
        {
            _eventCenter.Signal(eventData);
        }
    }

    public void Publish(EventData eventData)
    {
        _eventDatas.Add(eventData);
    }

    #endregion

}

public abstract class EventData
{
    protected EventData(string grainId)
    {
        EventId = Guid.NewGuid().ToString("N");
        GrainId = grainId;
    }

    #region Properties

    public string EventId { get; }

    public abstract string EventName { get; }

    public string GrainId { get; set; }

    #endregion

}
