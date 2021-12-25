using System;
using System.Diagnostics;

namespace GameImpl {
    public static class Program {
        [STAThread]
        static void Main() {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
            Trace.Listeners.Add(tr1);

            using (var game = new Game1())
                game.Run();
        }
    }
}
