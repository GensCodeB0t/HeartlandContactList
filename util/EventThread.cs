using System;
using System.Threading;
using System.Threading.Tasks;

namespace Heartland.util{
    public class EventThread : IDisposable{

        private Task _thread;
        private EventWaitHandle _StopTask = new EventWaitHandle(false, EventResetMode.ManualReset);

        private void Listener(EventWaitHandle waitHandle, EventHandler callback)
        {
            do
            {
                waitHandle.WaitOne();
                
                EventHandler _callback = callback;
                if (_callback != null)
                    _callback.Invoke(this, EventArgs.Empty);
            } while (!_StopTask.WaitOne(1));
        }

        public void StartListening(EventWaitHandle handle, EventHandler callback)
        {
            _thread = Task.Run(() => Listener(handle, callback));
        }

        public void StopListening()
        {   
            _StopTask.Set();
            _StopTask.Dispose();
        }

        public void Dispose()
        {
            _StopTask.Set();
            _StopTask.Dispose();
        }
    }
}
