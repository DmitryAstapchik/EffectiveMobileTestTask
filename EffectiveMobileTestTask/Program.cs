using EffectiveMobileTestTask;
using System.Net;
using System.Text.RegularExpressions;

// if no parameters are passed through the command line, the parameters will be taken from App.config

try
{
    var (FileLog, FileOutput, AddressStart, AddressMask, TimeStart, TimeEnd) = ParametersProvider.GetParameters(args);
    LogProcessor.ProcessLogFile(FileLog, FileOutput, TimeStart, TimeEnd, AddressStart, AddressMask);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Done.\nOutput file: {FileOutput}.");
    Console.ForegroundColor = ConsoleColor.Gray;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Message);
    Console.ForegroundColor = ConsoleColor.Gray;
}