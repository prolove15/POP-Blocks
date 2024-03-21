using System;

namespace POPBlocks.MapScripts
{
    public class LevelReachedEventArgs : EventArgs
    {
        public int Number
        {
            get; private set;
        }

        public LevelReachedEventArgs(int number)
        {
            Number = number;
        }
    }
}
