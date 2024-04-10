using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask
{
    public static class ParametersProvider
    {
        // if no parameters are passed through the command line, the parameters will be taken from App.config
        public static (string FileLog, string FileOutput, string? AddressStart, string? AddressMask, string TimeStart, string TimeEnd) GetParameters(string[] args)
        {
            string? fileLog, fileOutput, addressStart, addressMask, timeStart, timeEnd;
            fileLog = fileOutput = addressStart = addressMask = timeStart = timeEnd = null;
            string[] parameters = ["--file-log", "--file-output", "--address-start", "--address-mask", "--time-start", "--time-end"];

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length - 1; i++)
                {
                    string? argValue = !parameters.Any(p => p == args[i + 1]) ? args[i + 1] : null;
                    switch (args[i])
                    {
                        case "--file-log":
                            fileLog = argValue ?? fileLog;
                            break;
                        case "--file-output":
                            fileOutput = argValue ?? fileOutput;
                            break;
                        case "--address-start":
                            addressStart = argValue ?? addressStart;
                            break;
                        case "--address-mask":
                            addressMask = argValue ?? addressMask;
                            break;
                        case "--time-start":
                            timeStart = argValue ?? timeStart;
                            break;
                        case "--time-end":
                            timeEnd = argValue ?? timeEnd;
                            break;
                        default:
                            break;
                    }
                    if (argValue != null)
                    {
                        i++;
                    }
                }
            }
            else
            {
                fileLog = ConfigurationManager.AppSettings.Get("fileLog");
                fileOutput = ConfigurationManager.AppSettings.Get("fileOutput");
                addressStart = ConfigurationManager.AppSettings.Get("addressStart");
                addressMask = ConfigurationManager.AppSettings.Get("addressMask");
                timeStart = ConfigurationManager.AppSettings.Get("timeStart");
                timeEnd = ConfigurationManager.AppSettings.Get("timeEnd");
            }

            if (fileLog is null)
            {
                throw new ApplicationException($"Required parameter {parameters[0]} not specified.");
            }
            if (fileOutput is null)
            {
                throw new ApplicationException($"Required parameter {parameters[1]} not specified.");
            }
            if (timeStart is null)
            {
                throw new ApplicationException($"Required parameter {parameters[4]} not specified.");
            }
            if (timeEnd is null)
            {
                throw new ApplicationException($"Required parameter {parameters[5]} not specified.");
            }
            if (addressStart is null && addressMask is not null)
            {
                throw new ApplicationException($"Parameter {parameters[3]} cannot be used without specified {parameters[2]}.");
            }

            return (fileLog, fileOutput, addressStart, addressMask, timeStart, timeEnd);
        }
    }
}
