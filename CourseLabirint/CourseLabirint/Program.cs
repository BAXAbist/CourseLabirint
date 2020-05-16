using System;
using System.Collections.Generic;

namespace CourseLabirint
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1(new List<Game1.Cells>(), new Stack<Game1.Cells>()))
            {
                game.Run();
            }
        }
    }
#endif
}

