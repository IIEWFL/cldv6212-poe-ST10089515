using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionB_MessageQueue
{
    internal class Capture
    {

        public static void CaptureDetails()
        {
            Console.WriteLine("Enter details for vaccination:");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("ID: ");
            string id = Console.ReadLine();

            Console.Write("Center: ");
            string center = Console.ReadLine();

            Console.Write("Vaccination Date (YYYY/MM/DD): ");
            string vaccinationDate = Console.ReadLine();

            Console.Write("Serial Number: ");
            string serialNumber = Console.ReadLine();

            // Process captured details or store them as needed...
            Log.Information("Details captured successfully.");
        }




    }
}
