﻿using System;

namespace SFA.DAS.FAT.MockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Mock Server starting on http://localhost:5003");

            MockApiServer.Start();

            Console.WriteLine(("Press any key to stop the server"));
            Console.ReadKey();
        }
    }
}
