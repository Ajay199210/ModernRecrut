using System.Net;

namespace ModernRecrut.MVC.Services
{
    public static class AdresseIpGetter
    {
        public static string ObtenirAdresseIp()
        {
            string hostname = Dns.GetHostName();
            string Ip = Dns.GetHostByName(hostname).AddressList[0].MapToIPv4().ToString();
            return Ip;
        }
    }
}
