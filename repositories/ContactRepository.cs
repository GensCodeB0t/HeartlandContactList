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
        public ContactRepository(IDbContext database)
        {
            _database = database;
        }

        ///<summary>
        /// Adds a contact to the DbContext
        ///</summary>
        public async Task Add(Contact t)
        {
            await _database.Contacts.Add(t);
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