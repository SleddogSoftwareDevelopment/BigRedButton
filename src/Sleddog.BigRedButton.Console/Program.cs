using System;

namespace Sleddog.BigRedButton.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var button = new BigRedButton();

            System.Console.WriteLine("Listening");

            button.Listen()
                .Subscribe(state => System.Console.WriteLine(state));

            System.Console.WriteLine("Press enter to exit!");

            System.Console.ReadLine();
        }
    }
}