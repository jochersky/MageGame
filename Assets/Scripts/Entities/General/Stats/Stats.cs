using UnityEngine;

public enum StatType
{
    Health,
    Jumps,
    Speed
}

public class Stats
{
    readonly StatsMediator _mediator;
    readonly BaseStats _baseStats;
    
    public StatsMediator Mediator => _mediator;

    public float Health
    {
        get
        {
            var q = new Query(StatType.Health, _baseStats.health);
            _mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public float Jumps
    {
        get
        {
            var q = new Query(StatType.Jumps, _baseStats.jumps);
            _mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public float Speed
    {
        get
        {
            var q = new Query(StatType.Speed, _baseStats.speed);
            _mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public Stats(StatsMediator mediator, BaseStats baseStats)
    {
        _mediator = mediator;
        _baseStats = baseStats;
    }

    public override string ToString()
    {
        return $"Health: {Health}, Jumps: {Jumps}, Speed: {Speed}";
    }
}
