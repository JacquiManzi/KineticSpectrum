﻿using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace KinectTest
{
    class KinectTest
    {
        static void Main(string[] args)
        {
            args.Initialize();
            const string byteString1 = "0401dc4a01000801000000000000000001ef00000002f0ff0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            const string byteString2 = "0401dc4a01000801000000000000000002ef00000002f0ffff000000ff00ff00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            
            UdpClient client = new UdpClient(57692);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("169.254.49.154"), 6038);

            client.Connect(endPoint);

            byte[] buf = DecodeString(byteString1);
            client.Send(buf, buf.Length);

            buf = DecodeString(byteString2);
            client.Send(buf, buf.Length);

            client.Close();
        }

        public static byte[] DecodeString(String hexString)
        {
            byte[] hexBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length / 2; i += 2)
            {
                hexBytes[i/2] = Byte.Parse(hexString.Substring(i, 2).ToUpper(), NumberStyles.AllowHexSpecifier);
            }
            return hexBytes;
        }
    }
}
