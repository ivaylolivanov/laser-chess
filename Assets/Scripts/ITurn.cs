public enum Turn : uint {
    PLAYER = 1 << 0,
    ENEMY  = 1 << 1,
    WON    = 1 << 2,
    LOST   = 1 << 3,

    ALL_TURNS = LOST
}

public interface ITurn {
    public Turn Actions();
}
