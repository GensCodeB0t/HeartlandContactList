using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heartland.contracts;
using Heartland.data;
using Heartland.models;
using Heartland.util;

namespace Heartland.repositories{
    public class ContactRepository : IContactRepository
    {
        private IDbContext _database;
        private EventThread thread;
        public event EventHandler UpdateCountEvent;

        // TODO: Future realtime update to count
        private EventWaitHandle UpdateCountHandle;
        public ContactRepository(IDbContext database)
        {
            _database = database;

    #region  Future realtime update to count
            if(UpdateCountHandle == null){
                const string  EVENT_NAME = "Heartland.UpdateCountEvent";
                EventWaitHandle.TryOpenExisting(EVENT_NAME, out UpdateCountHandle);
                if(UpdateCountHandle == null)
                    UpdateCountHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EVENT_NAME);
                
                thread = new EventThread();
            }
    #endregion
        }
        public async Task Add(Contact t)
        {
            await _database.Contacts.Add(t);
            UpdateCountHandle.Set();
        }
        public Task<int> GetCount()
        {
            return _database.Contacts.GetCount();
        }

#region "FUTURE REALTIME UPDATE TO COUNT"
        public void StartListeningForUpdateCount(){
            if(thread != null)
                thread.StartListening(UpdateCountHandle, UpdateCountEvent);
        }

        public void StopListeningForUpdateCount(){
            if(thread != null)
                thread.StopListening();
        }

        public void Dispose()
        {
            thread.StopListening();
        }
    }
#endregion
}