using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CheckRegisterServiceLib;

namespace CheckRegisterServiceHost
    {
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;
            try
            {
                Console.WriteLine("Starting service...");
                host = new ServiceHost(typeof(CheckRegisterService));
                host.Open();
                Console.WriteLine("Service started.");
                Console.WriteLine("Press <ENTER> to stop service...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                Console.WriteLine("An error occurred starting the service: {0}", ex.Message);
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }

            Console.WriteLine("Press <ENTER> to quit...");
            Console.ReadLine();
        }
    }
    }
