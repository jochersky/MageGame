using System;

public enum OperatorTypes
{
    Add,
    Multiply
}

public class BasicStatModifier : StatModifier
{
    private string _source;
    private readonly StatType _type;
    private readonly Func<float, float> _operation;

    public BasicStatModifier(StatType statType, float duration, Func<float, float> operation, string source, bool stackableSource = false) : base(duration, source, stackableSource)
    {
        _type = statType;
        _operation = operation;
    }

    public override void Handle(object sender, Query query)
    {
        if (query.StatType == _type)
        {
            query.Value = _operation(query.Value);
        }
    }
}

public abstract class StatModifier : IDisposable
{
    public string Source { get; private set; }
    public bool MarkedForRemoval { get; private set; }
    public bool StackableSource { get; private set; }
    
    public event Action<StatModifier> OnDispose = delegate { };

    private readonly CountdownTimer timer;

    protected StatModifier(float duration, string source, bool stackableSource = false)
    {
        // permanent modifiers have 0/negative value
        if (duration <= 0) return;
        
        Source = source;
        StackableSource = stackableSource;
        timer = new CountdownTimer(duration);
        timer.OnTimerStop += Dispose;
        timer.Start();
    }
    
    public void Update(float deltaTime) => timer?.Tick(deltaTime);

    public void Reset() => timer?.Reset();
    
    public abstract void Handle(object sender, Query query);

    public void Dispose()
    {
        MarkedForRemoval = true;
        OnDispose?.Invoke(this);
    }
}