namespace Game.Audio
{
    // Which of an AudioEmitter's parallel AudioSources a sound plays on. A handful of fixed
    // channels (not a pool) on purpose: within a channel, a new Play() intentionally cuts off
    // whatever that channel was playing (e.g. one footstep replacing the previous one) — only
    // sounds on *different* channels are meant to be able to overlap without cutting each other
    // off (e.g. an attack bark shouldn't cut off footsteps, and vice versa).
    public enum AudioChannel
    {
        General,
        Movement,
        Action,
    }
}
