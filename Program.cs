using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace MaciejKozieja.Lang.JADE {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                WriteLine("Please enter file.");
                return;
            }
            if (args[0] == "/?") {
                // Display help
                WriteLine("JADE [file1 file2]");
                WriteLine("   translate files .jade to .html files with same name");
                WriteLine();
                WriteLine("JADE [-w][file1 file2]");
                WriteLine("   add wath to files when modyfied autoamticly translate them");
                WriteLine();
                WriteLine("JADE [-o][input>output]");
            } else if (args[0] == "/info") {
                // Display program info
                WriteLine("JADE translator by Maciej Kozieja");
                WriteLine("Project website https://github.com/koziejka/JADE");
                WriteLine("version: 1.0.0");
                WriteLine();
                WriteLine($@"File location: {Environment.CurrentDirectory}\JADE.exe");
            } else {
                // 
                if (args.Contains("-w")) {
                    // TODO implement file system wather 
                    WriteLine("file system wather not implemented yet.");
                } else if (args.Contains("-o")) {
                    // TODO implement file output changer
                    WriteLine("file output changer not implemented yet.");
                } else {
                    // translate files
                    for (int i = 0; i < args.Length; i++) {
                        if (Path.GetExtension(args[i]).ToLower() == ".jade")
                            JADE.Translate(args[i]);
                    }
                }
            }
        }
    }
}
