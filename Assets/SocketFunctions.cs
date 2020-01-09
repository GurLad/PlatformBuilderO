using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class SocketFunctions
{
    public static string IPString { get; set; } = "127.0.0.1";
    public static Socket Connect()
    {
        try
        {

            // Establish the remote endpoint  
            // for the socket. This example  
            // uses port 11111 on the local  
            // computer. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IPString), 4242);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket sender = new Socket(ipAddr.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            try
            {

                // Connect Socket to the remote  
                // endpoint using method Connect() 
                sender.Connect(localEndPoint);

                // We print EndPoint information  
                // that we are connected 
                Debug.Log("Socket connected to -> " +
                              sender.RemoteEndPoint.ToString());
                return sender;
            }

            // Manage of Socket's Exceptions 
            catch (System.ArgumentNullException ane)
            {

                Debug.LogError("ArgumentNullException : " + ane.ToString());
            }

            catch (SocketException se)
            {

                Debug.LogError("SocketException : " + se.ToString());
            }

            catch (System.Exception e)
            {
                Debug.LogError("Unexpected exception : " + e.ToString());
            }
        }

        catch (System.Exception e)
        {

            Debug.LogError(e.ToString());
        }
        return null;
    }
    public static void CloseSocket(this Socket sender)
    {
        // Close Socket using  
        // the method Close() 
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }
    public static void SendOne(this Socket sender, string data)
    {
        byte[] messageSent = Encoding.UTF8.GetBytes(data.Length.ToString() + '\a' + data);
        int byteSent = sender.Send(messageSent);
    }
    public static string RecieveOne(this Socket sender)
    {
        // Data buffer 
        byte[] messageReceived = new byte[1024];

        // We receive the messagge using  
        // the method Receive(). This  
        // method returns number of bytes 
        // received, that we'll use to  
        // convert them to string 
        int numFilled = 0;
        do
        {
            sender.Receive(messageReceived, numFilled, 1, SocketFlags.None);
        } while (messageReceived[numFilled++] != '\a');
        int size = int.Parse(Encoding.UTF8.GetString(messageReceived, 0, numFilled - 1));
        Debug.Log("Size: " + size);
        if (size == 0)
        {
            return "";
        }
        sender.Receive(messageReceived, size, SocketFlags.None);
        string messageString = Encoding.UTF8.GetString(messageReceived,0,size);
        Debug.Log(messageString.Length - 1);
        //messageString = messageString.Substring(0, 4);
        Debug.Log("Message from Server -> " +
              messageString);
        return messageString;
    }
    public static void SendLargeData(this Socket sender, string data)
    {
        int trueSize = Mathf.CeilToInt(data.Length / 1024.0f);
        sender.SendOne(trueSize.ToString());
        while (data.Length > 1024)
        {
            Debug.Log("Sent:\r\n" + data.Substring(0, 1024));
            sender.SendOne(data.Substring(0, 1024));
            data = data.Substring(1024);
        }
        Debug.Log("Sent:\r\n" + data);
        sender.SendOne(data);
    }
    public static string ReceiveLargeData(this Socket sender)
    {
        int fileSize = int.Parse(sender.RecieveOne());
        string result = "";
        for (int i = 0; i < fileSize; i++)
        {
            result += sender.RecieveOne();
        }
        return result;
    }
    public static string HexToIP(string hex)
    {
        string result = "";
        for (int i = 0; i < 4; i++)
        {
            result += int.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber) + ".";
        }
        return result.Substring(0, result.Length - 1);
    }
    public static string IPToHex(string ip)
    {
        string result = "";
        string[] ipParts = ip.Split('.');
        for (int i = 0; i < 4; i++)
        {
            result += int.Parse(ipParts[i]).ToString("X");
        }
        return result;
    }
}
