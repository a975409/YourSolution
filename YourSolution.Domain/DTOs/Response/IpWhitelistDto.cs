using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.DTOs.Response
{
    public class IpWhitelistDto
    {
        public int Id { get; set; }
        public bool IsEnable { get; set; } = true;
        public string IpAddress { get; set; } = string.Empty;
        public int Mask { get; set; } = 32;
        public bool IsEditing { get; set; }
        public string Part1 { get; set; } = string.Empty;
        public string Part2 { get; set; } = string.Empty;
        public string Part3 { get; set; } = string.Empty;
        public string Part4 { get; set; } = string.Empty;
        public bool IsIpInSubnet(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress).GetAddressBytes();
            var subnetIp = IPAddress.Parse(IpAddress).GetAddressBytes();
            var subnetMask = GenerateSubnetMask(Mask);

            if (ip.Length != subnetIp.Length || subnetIp.Length != subnetMask.Length)
            {
                return false;
            }

            for (int i = 0; i < ip.Length; i++)
            {
                if ((ip[i] & subnetMask[i]) != (subnetIp[i] & subnetMask[i]))
                {
                    return false;
                }
            }

            return true;
        }
        private static byte[] GenerateSubnetMask(int prefixLength)
        {
            byte[] mask = new byte[4];

            int fullBytes = prefixLength / 8;
            int partialBits = prefixLength % 8;

            for (int i = 0; i < fullBytes; i++)
            {
                mask[i] = 0xFF;
            }

            if (partialBits > 0)
            {
                mask[fullBytes] = (byte)(0xFF << 8 - partialBits);
            }

            return mask;
        }
    }
}
