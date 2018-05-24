// This is a simple TCP and UDP client application. For TCP, the server name is
// resolved and a socket is created to attempt a connection to each address
// returned until a connection succeeds. Once connected the client sends a "request"
// message to the server and shuts down the sending side. The client then loops
// to receive the server response until the server closes the connection at which
// point the client closes its socket and exits. For UDP, the server name is resolved
// and the first address returned is used (since there is no indication that there
// is a UDP server at the endpoint). The UDP client then sends a single datagram
// "request" message and then waits to receive a response from the server. The client
// continues to receive until a zero byte datagram is received. Note that the server
// sends several zero byte datagrams but if they are lost, the client will never
// exit.
//

// usage:
//      Executable_file_name [-c] [-n server] [-p port] [-m message]
//                       [-t tcp|udp] [-x size]
//           -c                         If UDP connect the socket before sending
//           -n server             Server name or address to connect/send to
//           -p port                 Port number to connect/send to
//           -m message      String message to format in request buffer
//           -t tcp|udp            Indicates to use either the TCP or UDP protocol
//           -x size                 Size of send and receive buffers
//

// sample usage:
//      The following command line establishes a TCP connection to the given server
//      on port 5150. The other two command lines are sample server command lines --
//      one for IPv4 and one for IPv6. Since the client will attempt to resolve
//      the server's name, it should attempt to connect over IPv4 and IPv6 as long
//      as the addresses are registered in DNS.
//

//          Executable_file_name -n server -p 5150 -t tcp
//          Executable_file_name -l :: -p 5150 -t tcp
//          Executable_file_name -l 0.0.0.0 -p 5150 -t tcp

//
//      The following command line creates a connected UDP socket that sends
//      data to the server x.y.z.w on port 5150. While the second entry is the
//      server command line used.
//      NOTE: For UDP sockets, the buffer size on the client and server should match
//            as an exception will be thrown if the receiving buffer is smaller than
//            the incoming datagram.
//

//          Executable_file_name -n x.y.z.w -p 5150 -t udp -c -x 512
//          Executable_file_name -l x.y.z.w -p 5150 -t udp -x 512
//

using System;
using System.Net;
using System.Net.Sockets;
using DNP3Simple;
using System.Text;

/// <summary>
/// This is a simple TCP and UDP based client.
/// </summary>

namespace SocketClient
{
    class ClientSocket
    {
        /// <summary>
        /// This routine repeatedly copies a string message into a byte array until filled.
        /// </summary>
        /// <param name="dataBuffer">Byte buffer to fill with string message</param>
        /// <param name="message">String message to copy</param>

        static public void FormatBuffer(byte[] dataBuffer, byte[] byteMessage)
        {
            //byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(message);
            int index = 0;
            // First convert the string to bytes and then copy into send buffer
            while (index < dataBuffer.Length)
            {
                for (int j = 0; j < byteMessage.Length; j++)
                {
                    dataBuffer[index] = byteMessage[j];
                    index++;
                    // Make sure we don't go past the send buffer length
                    if (index >= dataBuffer.Length)
                    {
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Prints simple usage information.
        /// </summary>
        static public void usage()
        {
            Console.WriteLine("usage: Executable_file_name [-c] [-n server] [-p port] [-m message]");
            Console.WriteLine("                        [-t tcp|udp] [-x size]");
            Console.WriteLine("     -c              If UDP connect the socket before sending");
            Console.WriteLine("     -n server       Server name or address to connect/send to");
            Console.WriteLine("     -p port         Port number to connect/send to");
            Console.WriteLine("     -m message      String message to format in request buffer");
            Console.WriteLine("     -t tcp|udp      Indicates to use either the TCP or UDP protocol");
            Console.WriteLine("     -x size         Size of send and receive buffers");
            Console.WriteLine(" Else, default values will be used...");

        }



        /// <summary>
        /// This is the main function for the simple client. It parses the command line and creates
        /// a socket of the requested type. For TCP, it will resolve the name and attempt to connect
        /// to each resolved address until a successful connection is made. Once connected a request
        /// message is sent followed by shutting down the send connection. The client then receives
        /// data until the server closes its side at which point the client socket is closed. For
        /// UDP, the socket is created and if indicated connected to the server's address. A single
        /// request datagram message. The client then waits to receive a response and continues to
        /// do so until a zero byte datagram is receive which indicates the end of the response.
        /// </summary>

        /// <param name="args">Command line arguments</param>

        static void Main(string[] args)
        {
            SocketType sockType = SocketType.Stream;
            ProtocolType sockProtocol = ProtocolType.Tcp;
            string remoteName = "localhost";
            string textMessage = "Client: This is a test";
            bool udpConnect = false;
            int remotePort = 5150, bufferSize = 4096;
            Console.WriteLine();
            usage();
            Console.WriteLine();
            // Parse the command line
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if ((args[i][0] == '-') || (args[i][0] == '/'))
                    {
                       switch (Char.ToLower(args[i][1]))
                        {
                            case 'c':       // "Connect" the UDP socket to the destination
                                udpConnect = true;
                                break;
                            case 'n':       // Destination address to connect to or send to
                                remoteName = args[++i];
                                break;
                            case 'm':       // Text message to put into the send buffer
                                textMessage = args[++i];
                                break;
                            case 'p':       // Port number for the destination
                                remotePort = System.Convert.ToInt32(args[++i]);
                                break;
                            case 't':       // Specified TCP or UDP
                                i++;
                                if (String.Compare(args[i], "tcp", true) == 0)
                                {
                                    sockType = SocketType.Stream;
                                    sockProtocol = ProtocolType.Tcp;
                                }
                                else if (String.Compare(args[i], "udp", true) == 0)
                                {
                                    sockType = SocketType.Dgram;
                                    sockProtocol = ProtocolType.Udp;
                                }
                                else
                                {
                                    usage();
                                    return;
                                }
                                break;
                            case 'x':       // Size of the send and receive buffers
                                bufferSize = System.Convert.ToInt32(args[++i]);
                                break;
                            default:
                                usage();
                                return;
                        }
                    }
                }
                catch
                {
                   usage();
                   return;
                }
            }

            Socket clientSocket = null;
            IPHostEntry resolvedHost = null;
            IPEndPoint destination = null;
            byte[] sendBuffer = new byte[bufferSize], recvBuffer = new Byte[bufferSize];
            int rc;
            // Format the string message into the send buffer

            // we will send the DNP3 request
            DNP3Simple.ApplicationLayer al = new DNP3Simple.ApplicationLayer();
            al.ApplicationData = new byte[] { 0x02, 0x00, 0x06};
            al.FunctionCode = 0x01;
            al.ApplicationControl = 0xc2;
            al.serialize(ref al.ApplicationData);

            DNP3Simple.TransportLayer tl = new DNP3Simple.TransportLayer();
            tl.TransportData = al.ApplicationData;
            tl.seq = 2;
            tl.FIN = 1;
            tl.FIR = 1;
            tl.serialize(ref tl.TransportData);

            DNP3Simple.LinkLayer ll = new DNP3Simple.LinkLayer();
            ll.LinkData = tl.TransportData;
            ll.source = 3;
            ll.destination = 4;
            ll.controlByte = 0xc4;
            ll.serialize(ref ll.LinkData);

            var sb = new StringBuilder("new byte[] { ");
            for (var i = 0; i < ll.LinkData.Length; i++)
            {
                var b = ll.LinkData[i];
                sb.Append(b);
                if (i < ll.LinkData.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            Console.WriteLine("DNP3 data using String Builder " +sb.ToString());

            //FormatBuffer(sendBuffer, textMessage);
            FormatBuffer(sendBuffer, ll.LinkData);
            try
            {
                // Try to resolve the remote host name or address
                resolvedHost = Dns.GetHostEntry(remoteName);
                Console.WriteLine("Client: GetHostEntry() is OK...");

                // Try each address returned
                foreach (IPAddress addr in resolvedHost.AddressList)
                {
                    // Create a socket corresponding to the address family of the resolved address
                    clientSocket = new Socket(
                        addr.AddressFamily,
                        sockType,
                        sockProtocol
                        );

                    Console.WriteLine("Client: Socket() is OK...");
                    try
                    {
                        // Create the endpoint that describes the destination
                        destination = new IPEndPoint(addr, remotePort);
                        Console.WriteLine("Client: IPEndPoint() is OK. IP Address: {0}, server port: {1}", addr, remotePort);
                        if ((sockProtocol == ProtocolType.Udp) && (udpConnect == false))
                       {
                            Console.WriteLine("Client: Destination address is: {0}", destination.ToString());
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Client: Attempting connection to: {0}", destination.ToString());
                        }
                        clientSocket.Connect(destination);
                        Console.WriteLine("Client: Connect() is OK...");
                        break;
                    }
                    catch (SocketException)
                    {
                        // Connect failed, so close the socket and try the next address
                        clientSocket.Close();
                        Console.WriteLine("Client: Close() is OK...");
                        clientSocket = null;
                        continue;
                    }
                }

                // Make sure we have a valid socket before trying to use it
                if ((clientSocket != null) && (destination != null))
                {
                    try
                    {
                        // Send the request to the server
                        if ((sockProtocol == ProtocolType.Udp) && (udpConnect == false))
                        {
                            clientSocket.SendTo(sendBuffer, destination);
                            Console.WriteLine("Client: SendTo() is OK...UDP...");
                        }
                        else
                        {
                            rc = clientSocket.Send(sendBuffer);
                            Console.WriteLine("Client: send() is OK...TCP...");
                            Console.WriteLine("Client: Sent request of {0} bytes", rc);

                            // For TCP, shutdown sending on our side since the client won't send any more data
                            if (sockProtocol == ProtocolType.Tcp)
                            {
                                clientSocket.Shutdown(SocketShutdown.Send);
                                Console.WriteLine("Client: Shutdown() is OK...");
                            }
                        }

                        // Receive data in a loop until the server closes the connection. For
                        //    TCP this occurs when the server performs a shutdown or closes
                        //    the socket. For UDP, we'll know to exit when the remote host
                       //    sends a zero byte datagram.
                        while (true)
                        {
                            if ((sockProtocol == ProtocolType.Tcp) || (udpConnect == true))
                            {
                                rc = clientSocket.Receive(recvBuffer);
                                Console.WriteLine("Client: Receive() is OK...");
                                Console.WriteLine("Client: Read {0} bytes", rc);
                            }
                            else
                            {
                                IPEndPoint fromEndPoint = new IPEndPoint(destination.Address, 0);
                                Console.WriteLine("Client: IPEndPoint() is OK...");
                                EndPoint castFromEndPoint = (EndPoint)fromEndPoint;
                                rc = clientSocket.ReceiveFrom(recvBuffer, ref castFromEndPoint);
                                Console.WriteLine("Client: ReceiveFrom() is OK...");
                                fromEndPoint = (IPEndPoint)castFromEndPoint;
                                Console.WriteLine("Client: Read {0} bytes from {1}", rc, fromEndPoint.ToString());
                            }
                            // Exit loop if server indicates shutdown
                            if (rc == 0)
                            {
                                clientSocket.Close();
                                Console.WriteLine("Client: Close() is OK...");
                                break;
                            }
                        }
                    }
                   catch (SocketException err)
                    {
                        Console.WriteLine("Client: Error occurred while sending or receiving data.");
                        Console.WriteLine("   Error: {0}", err.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Client: Unable to establish connection to server!");
                }
            }
            catch (SocketException err)
            {
                Console.WriteLine("Client: Socket error occurred: {0}", err.Message);
            }
        }
    }
}