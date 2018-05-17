
using System;

namespace FakeContract
{
    public interface ISimulator
    {
        void Show();
        void Close();
        event EventHandler Closed;
    }
}
