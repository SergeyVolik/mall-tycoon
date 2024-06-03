namespace Prototype
{
    public interface ICooldown
    {
        float Duration { get; set; }
        float Progress { get; }

        public bool IsFinished { get; }
        public bool IsPaused { get; }
        void Restart();
        void Stop();
        void Play();

        void Tick(float deltaTime);
    }
}
