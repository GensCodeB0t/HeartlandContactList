using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heartland.contracts;
using Heartland.data;
using Heartland.models;
using Heartland.util;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Heartland.repositories{
    public class ContactRepository : IContactRepository
    {
        public event EventHandler CountAddedEvent;
        private IDbContext _database;
        public ContactRepository(IDbContext database)
        {
            _database = database;
        }

        ///<summary>
        /// Adds a contact to the DbContext
        ///</summary>
        public async void Add(Contact t)
        {
            await _database.Contacts.Add(t);
            CountAddedEvent.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        /// Gets a contact count from the DbContext
        ///</summary>
        public Task<int> GetCount()
        {
            return _database.Contacts.GetCount();
        }

    }
}