// Class to register all game events

public class GameEvent { }

public class OnPause : GameEvent { }

public class OnCollect : GameEvent
{
    public int amount;
}

public class OnLevelUp : GameEvent { }

public class OnPunch : GameEvent
{
    public float duration;
    public float magnitude;
}