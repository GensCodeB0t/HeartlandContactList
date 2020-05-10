using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Heartland.contracts;

namespace Heartland.util{
    public static class CustomConsole{
        ///<summary>
        /// Clears all but the top line (used to display the count) of the console
        ///</summary>
        public static void ClearConsole(){
            for(var i = Console.CursorTop; i > 0; i--){
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth)); 
                Console.SetCursorPosition(0, i);
            }
        }

        ///<summary>
        /// Updates the top line (used to display the count) of the console
        ///</summary>
        public static void UpdateCount(int count){
            var curPosTop = Console.CursorTop;
            var curPosLeft = Console.CursorLeft;
            Console.SetCursorPosition(0, 0);
            Console.Write($"Contact count: {count}");
            Console.SetCursorPosition(curPosLeft, curPosTop);
        }
        

    }
}