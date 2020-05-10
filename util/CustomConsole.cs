using System;
using System.Threading.Tasks;
using Heartland.contracts;

namespace Heartland.util{
    public static class CustomConsole{
        public static void ClearConsole(){
            for(var i = Console.CursorTop; i > 0; i--){
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth)); 
                Console.SetCursorPosition(0, i);
            }
        }
        public static void UpdateCount(int count){
            var curPosTop = Console.CursorTop;
            var curPosLeft = Console.CursorLeft;
            Console.SetCursorPosition(0, 0);
            Console.Write($"Contact count: {count}");
            Console.SetCursorPosition(curPosLeft, curPosTop);
        }
        public static async Task<int> UpdateCount(IContactRepository contactRepository){
            int _count = await contactRepository.GetCount();
            UpdateCount(_count);
            return _count;
        }
        
    }
}