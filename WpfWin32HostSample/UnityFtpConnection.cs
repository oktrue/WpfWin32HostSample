using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfWin32HostSample
{
    /// <summary>
    /// https://forum.unity.com/threads/connect-unity-and-wpf.337945/
    /// </summary>
    internal class UnityFtpConnection
    {
        private bool receiving = true;
        Socket s;

        public void Test()
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");
                // use local m/c IP address, and
                // use the same in the client

                /* Initializes the Listener */
                TcpListener myList = new TcpListener(ipAd, 8001);

                /* Start Listeneting at the specified port */
                myList.Start();

                Debug.WriteLine("The server is running at port 8001...");
                Debug.WriteLine("The local End point is :" + myList.LocalEndpoint);
                Debug.WriteLine("Waiting for a connection...");

                s = myList.AcceptSocket();
                Debug.WriteLine("Connection accepted from " + s.RemoteEndPoint);
                while (receiving)
                {
                    byte[] b = new byte[100];

                    int k = s.Receive(b);

                    Debug.WriteLine("Received...");
                    string received = "";

                    for (int i = 0; i < k; i++)
                    {
                        received += Convert.ToChar(b[i]);
                    }

                    Debug.WriteLine(received);
                    ASCIIEncoding asen = new ASCIIEncoding();
                    s.Send(asen.GetBytes("Hey I Got This Msg: " + received));
                }
                Debug.WriteLine("Connection closing...");
                /* clean up */
                s.Close();
                myList.Stop();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error. " + e.StackTrace);
            }
        }

        public void Send(string message)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes("Hey I Got This Msg: " + message));
        }

        public void Stop()
        {
            receiving = false;
        }
    }
}
