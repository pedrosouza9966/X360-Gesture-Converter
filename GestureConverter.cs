using System;
using System.IO;
using System.Linq;

class GestureConverter
{
    static void Main(string[] args)
    {
        string inputFolder = "input";
        string outputFolder = "output";

        // Verifica se a pasta existe
        if (!Directory.Exists(inputFolder))
        {
            Console.WriteLine("Move your Xbox One gestures to the input folder and run the app again.");
            Directory.CreateDirectory(inputFolder);
            Console.ReadKey();
            return;
        }

        // Lista todos os gestures
        string[] gestureFiles = Directory.GetFiles(inputFolder, "*.gesture");

        // Verifica se tem pelo menos um gesture na pasta
        if (gestureFiles.Length == 0)
        {
            Console.WriteLine("You need at least one gesture on the input folder.");
            Console.ReadKey();
            return;
        }

        // Caso a pasta não exista…
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        int convertedGestures = 0;

        foreach (string gestureFile in gestureFiles)
        {
            byte[] fileBytes = File.ReadAllBytes(gestureFile);

            // Ignora o header do xone
            byte[] gestureData = fileBytes.Skip(22).ToArray();

            // Troca a endianidade de cada bloco de 4 bytes
            for (int i = 0; i < gestureData.Length; i += 4)
            {
                if (i + 4 <= gestureData.Length)
                {
                    Array.Reverse(gestureData, i, 4);
                }
            }

            string outputFilePath = Path.Combine(outputFolder, Path.GetFileName(gestureFile));
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create)))
            {
                // Escreve o header do gesture do x360
                writer.Write(System.Text.Encoding.ASCII.GetBytes("GestureDetectorX360"));

                // ??
                writer.Write((byte)0x00);
                writer.Write(gestureData);
            }

            convertedGestures++;
        }

        Console.WriteLine($"{convertedGestures} gestures converted!");
        Console.WriteLine("Press any key to exit…");
        Console.ReadKey();
    }
}
