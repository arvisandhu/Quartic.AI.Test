namespace Quartic.AI.Test.Interfaces
{
    using System;

    public interface IClosable
    {
        void Close();
        event EventHandler CloseTriggered;
    }
}