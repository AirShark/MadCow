using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace MadCow
{
    class Ping
    {
        private int pingsSent;
        // Can be used to notify when the operation completes
        AutoResetEvent resetEvent = new AutoResetEvent(false);

        public void StartPingTest()
        {
            pingsSent = 0;
            SendPing();
        }

        private void SendPing()
        {
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();

            pingSender.PingCompleted += new PingCompletedEventHandler(pingTest_Complete);

            // Create a buffer of 32 bytes.
            byte[] _data = Encoding.ASCII.GetBytes("................................");

            PingOptions Options = new PingOptions(50, true);

            // Send the ping asynchronously
            pingSender.SendAsync("www.google.com", 2000, _data, Options, resetEvent);
        }

        private void pingTest_Complete(object sender, PingCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ((AutoResetEvent)e.UserState).Set();
            }
            else
            {
                PingReply pingResponse = e.Reply;
                // Call the method that displays the ping results, and pass the information with it
                if (pingResponse.Status == IPStatus.Success)
                    Console.WriteLine("Internet connectivity found.");
                else
                    MessageBox.Show("MadCow was unable to detect an active Internet connection." + "\nMadCow has an strong Internet dependency and wont work properly under this conditions.", "[FATAL ERROR] - No Internet Connection was found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
