using System;
using System.Linq;
using System.Threading.Tasks;
using Heartland.models;

namespace Heartland.contracts{
    public interface IBaseRepository<T>{
        Task<int> GetCount();
        Task Add(T t);
    }
}