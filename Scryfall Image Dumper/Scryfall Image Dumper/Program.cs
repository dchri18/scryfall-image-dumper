using System;

namespace Scryfall_Image_Dumper {
    class Program {

        static void Main(string[] args) {

            Console.Write("----- Scryfall Image Dumper -----\n" +
                "Author: Daniel Christensen\n\n" +
                "This program was designed to be used on webpages from the scryfall website that list all cards\n" +
                "from a MTG set. The purpose is to get all high quality images for each card on to your computer quickly.\n" +
                "An example of a valid URL would be: www.scryfall.com/sets/m20\n");
            Console.Write("You can find the dump within the project folder.\n\n");
            Console.Write("Enter URL: ");

            string input = Console.ReadLine();
            using (Dumper dump = new Dumper(input)) {
                dump.PerformDump();
            }
            Console.WriteLine("Program finished.");
        }
    }
}
