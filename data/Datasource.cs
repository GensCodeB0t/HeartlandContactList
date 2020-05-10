using System.IO;
using System.Linq;
using System;
using System.Text.Json;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace Heartland.data{
    public class Datasource<T>{
        private static Mutex mut = new Mutex(false, "HearlandContactList");
        private string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            ,$"Heartland\\{typeof(T).Name}.data"
        );

        public Datasource()
        {
            if(!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        ///<summary>
        /// Serializes values to file
        ///</summary>
        ///<param name="values">IQueryable<T> of values to serialize</param>
        private async Task Serialize(IQueryable<T> values){
            try{
                mut.WaitOne();
                using (FileStream fs = File.Create(filePath))
                {
                    var options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    await JsonSerializer.SerializeAsync(fs, values, options);
                }
                mut.ReleaseMutex();
            } catch(AbandonedMutexException e) {
                // Notify the user and/or try again
            } catch(Exception e) {
                
            }
        }

        ///<summary>
        /// Deserialize file
        ///</summary>
        ///<returns>IQueryable<T> of values to from a file</returns>
        private async Task<IQueryable<T>> Deserialize(){
            if(File.Exists(filePath)){
                try{
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        var _values = await JsonSerializer.DeserializeAsync<T[]>(fs);
                        if(_values != null)
                            return _values.AsQueryable();
                    } 
                } catch(AbandonedMutexException e) {
                // Notify the user and/or try again
                } catch(Exception e) {
                    
                }
            }
            return null;
        }

        ///<summary>
        /// Adds a entity to the file
        ///</summary>
        ///<param name="t">Entity to add to the file (of type T)</param>
        ///<returns>Task</returns>
        public async Task Add(T t){
            var values = Enumerable.Empty<T>().Append(t).AsQueryable();
            IQueryable<T> _values = await Deserialize();
            if(_values != null)
                values = values.Concat(_values); 
            await Serialize(values);
        }

        ///<summary>
        /// Gets the number of entities in the file
        ///</summary>
        ///<returns>number of entities in the file</returns>
        public async Task<int> GetCount(){
            var values = await Deserialize();
            return values != null? values.Count() : 0;
        }
    }
}