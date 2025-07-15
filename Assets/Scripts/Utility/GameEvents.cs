// Class to register all game events

public class GameEvent { }

public class OnPause : GameEvent { }

public class OnLevelUp : GameEvent { }

public class OnXpGain : GameEvent
{
    public int amount;
}

public class OnCollect : GameEvent
{
    public int amount;
}

public class OnDropCorpse : GameEvent
{
    public int id;
}

public class OnPunch : GameEvent
{
    public float duration;
    public float magnitude;
}