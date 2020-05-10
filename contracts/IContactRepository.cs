using System;
using System.Linq;
using System.Threading;
using Heartland.models;

namespace Heartland.contracts{
    public interface IContactRepository : IDisposable, IBaseRepository<Contact>{
        void StartListeningForUpdateCount();
        void StopListeningForUpdateCount();
        event EventHandler UpdateCountEvent;
    } 
    
}