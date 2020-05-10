using System.IO;
using System.Linq;
using System;
using System.Text.Json;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace Heartland.data{
    public class Datasource<T>{
        private IQueryable<T> Data { get; set; }
        private static Mutex mut = new Mutex(false, "HearlandContactList");
        private string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            ,$"Heartland\\{typeof(T).Name}.data"
        );

        public Datasource()
        {
            if(!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
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

        public async Task Add(T t){
            var values = Enumerable.Empty<T>().Append(t).AsQueryable();
            IQueryable<T> _values = await Deserialize();
            if(_values != null)
                values = values.Concat(_values); 
            await Serialize(values);
        }

        public async Task<int> GetCount(){
            var values = await Deserialize();
            return values != null? values.Count() : 0;
        }
    }
}