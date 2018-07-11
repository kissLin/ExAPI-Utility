using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lion.ExAPI.Utility.Extensions {

    /// <summary>
    /// 抓取IP 擴充
    /// </summary>
    public static class IPAddressExtension {

        /// <summary>Ipv4 轉 長整數</summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static UInt32 ParseToInt64(this IPAddress ip) {

            return (UInt32)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ip.GetAddressBytes(), 0));
        }


        /// <summary>比對IP使否為虛擬(內網)IP</summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Boolean IsIntraNet(this IPAddress ip) {

            var ipValue = ip.ParseToInt64();

            return privateRanges.Any((i) => {
                return ipValue >= i[0].ParseToInt64() && ipValue <= i[1].ParseToInt64();
            });

        }



        /// <summary>Private IPs Ranges</summary>
        private static readonly IPAddress[][] privateRanges = new IPAddress[][]{
            new IPAddress [] { IPAddress.Parse("127.0.0.1") , IPAddress.Parse("127.0.0.1") },
            new IPAddress [] { IPAddress.Parse("10.0.0.0") , IPAddress.Parse("10.255.255.255") },
            new IPAddress [] { IPAddress.Parse("172.16.0.0" ), IPAddress.Parse("172.31.255.255") },
            new IPAddress [] { IPAddress.Parse("192.168.0.0") , IPAddress.Parse("192.168.255.255")}
        };

    }
}
