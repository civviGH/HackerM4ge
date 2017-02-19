namespace WandAction
{
    public class Vibrate
    {
        public ushort microseconds { get; private set; }

        public Vibrate(ushort microseconds)
        {
            this.microseconds = microseconds;
        }
    }
}
