// Class to register all game events

public class GameEvent { }

public class OnPause : GameEvent { }

public class OnEndGame : GameEvent { }

public class OnStoreInteraction : GameEvent { }

public class OnLevelUp : GameEvent
{
    public int strength;
    public float speed;
}

public class OnExperienceChange : GameEvent
{
    public int previous;
    public int delta;
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