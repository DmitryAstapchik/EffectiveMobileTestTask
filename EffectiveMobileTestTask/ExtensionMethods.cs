using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask
{
    internal static class ExtensionMethods
    {
        internal static bool IsInRange(this IPAddress address, IPAddress start, IPAddress mask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] startIpBytes = start.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();

            if (ipAdressBytes.Length != startIpBytes.Length || startIpBytes.Length != maskBytes.Length)
                throw new ArgumentException("Lengths of IP address, start IP and subnet mask do not match.");

            uint ipNum = BitConverter.ToUInt32(ipAdressBytes.Reverse().ToArray(), 0);
            uint startIpNum = BitConverter.ToUInt32(startIpBytes.Reverse().ToArray(), 0);
            uint maskNum = BitConverter.ToUInt32(maskBytes.Reverse().ToArray(), 0);

            uint endIpNum = startIpNum + ~maskNum;

            return ipNum >= startIpNum && ipNum <= endIpNum;
        }
    }
}
