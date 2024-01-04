using System.Net;

namespace ModernRecrut.Favoris.API.Services
{
    public static class AdresseIpGetter
    {
        // Client REMOTE address should be used instead of the local one which is used for testing purposes
        // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
        // https://www.c-sharpcorner.com/UploadFile/scottlysle/getting-an-external-ip-address-locally/
        public static string ObtenirAdresseIp()
        {
            string hostname = Dns.GetHostName();
            string Ip = Dns.GetHostByName(hostname).AddressList[0].MapToIPv4().ToString();

            return Ip;
        }
    }
}