using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EffectiveMobileTestTask
{
    public class LogProcessor
    {
        public static void ProcessLogFile(string fileLog, string fileOutput, string timeStart, string timeEnd, string? addressStart = null, string? addressMask = null)
        {
            var allVisits = GetIpVisits(fileLog);
            var visitsInRange = GetIpVisitsInRange(allVisits, timeStart, timeEnd, addressStart, addressMask);
            WriteToFile(visitsInRange, fileOutput);
        }

        private static IEnumerable<IpVisit> GetIpVisits(string fileLog)
        {
            if (!string.IsNullOrWhiteSpace(fileLog))
            {
                if (!File.Exists(fileLog))
                {
                    throw new ArgumentException($"File {fileLog} does not exist.");
                }
            }
            else
            {
                throw new ArgumentException($"{nameof(fileLog)} cannot be empty.");
            }

            var fileText = File.ReadAllText(fileLog);

            string ipv4Pattern = @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            string dateTimePattern = @"\b\d{4}-(0?[1-9]|1[0-2])-(0?[1-9]|[12][0-9]|3[01]) (00|[0-9]|[01][0-9]|2[0-3]):([0-9]|[0-5][0-9]):([0-9]|[0-5][0-9])\b";
            var regex = new Regex(@$"{ipv4Pattern}\s*:\s*{dateTimePattern}");
            var visits = regex.Matches(fileText).Select(m => new IpVisit
            {
                IpAddress = IPAddress.Parse(Regex.Match(m.Value, ipv4Pattern).Value),
                VisitTime = DateTime.Parse(Regex.Match(m.Value, dateTimePattern).Value)
            });
            return visits;
        }

        private static IEnumerable<IpVisit> GetIpVisitsInRange(IEnumerable<IpVisit> visits, string timeStart, string timeEnd, string? addressStart = null, string? addressMask = null)
        {
            if (!DateOnly.TryParse(timeStart, out DateOnly timeStartDate))
            {
                throw new ArgumentException($"{timeStart} is not a valid date.");
            }

            if (!DateOnly.TryParse(timeEnd, out DateOnly timeEndDate))
            {
                throw new ArgumentException($"{timeEnd} is not a valid date.");
            }

            if (timeStartDate > timeEndDate)
            {
                throw new ArgumentException($"{nameof(timeStart)} is greater than {nameof(timeEnd)}.");
            }

            IPAddress? ipAddressStart = null;
            if (!string.IsNullOrEmpty(addressStart))
            {
                if (!(IPAddress.TryParse(addressStart, out ipAddressStart) && ipAddressStart.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                {
                    throw new ArgumentException($"{addressStart} is not a valid IPv4 address.");
                }
            }

            IPAddress? ipAddressMask = null;
            if (!string.IsNullOrEmpty(addressMask))
            {
                if (ipAddressStart != null)
                {
                    if (!IPAddress.TryParse(addressMask, out ipAddressMask) && ipAddressMask?.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        throw new ArgumentException($"{addressMask} is not a valid IPv4 address.");
                    }
                }
                else
                {
                    throw new ArgumentException($"You cannot use {nameof(addressMask)} without valid {nameof(addressStart)}.");
                }
            }

            var visitsInRange = visits.Where(v => ipAddressStart != null && ipAddressMask != null ?
                                            v.IpAddress!.IsInRange(ipAddressStart, ipAddressMask) :
                                            ipAddressStart == null || v.IpAddress!.Address >= ipAddressStart.Address)
                                      .Where(v => DateOnly.FromDateTime(v.VisitTime) >= timeStartDate && DateOnly.FromDateTime(v.VisitTime) <= timeEndDate);
            return visitsInRange;
        }

        private static void WriteToFile(IEnumerable<IpVisit> visits, string fileOutput)
        {
            if (string.IsNullOrWhiteSpace(fileOutput) || fileOutput.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException($"{fileOutput} is an invalid file path.");
            }

            string fileContents = string.Join('\n', visits.GroupBy(v => v.IpAddress).Select(g => $"{g.Key} : {g.Count()}"));
            File.WriteAllText(fileOutput, fileContents);
        }

        public class IpVisit
        {
            public IPAddress? IpAddress { get; set; }
            public DateTime VisitTime { get; set; }
        }
    }
}
