using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectiveMobileTestTask.Tests
{
    [TestFixture]
    public class ParametersProviderTests
    {
        private const string testFileLog = "testLogFile.txt";
        private const string testFileOutput = "testOutputFile.txt";
        private const string testAddressStart = "1.1.1.1";
        private const string testAddressMask = "128.0.0.0";
        private const string testTimeStart = "01.01.2024";
        private const string testTimeEnd = "31.12.2024";

        [Test]
        public void GetParameters_ArgsLengthIsZero_ReturnsAllParametersFromAppSettings()
        {
            ConfigurationManager.AppSettings.Set("fileLog", testFileLog);
            ConfigurationManager.AppSettings.Set("fileOutput", testFileOutput);
            ConfigurationManager.AppSettings.Set("addressStart", testAddressStart);
            ConfigurationManager.AppSettings.Set("addressMask", testAddressMask);
            ConfigurationManager.AppSettings.Set("timeStart", testTimeStart);
            ConfigurationManager.AppSettings.Set("timeEnd", testTimeEnd);

            var (FileLog, FileOutput, AddressStart, AddressMask, TimeStart, TimeEnd) = ParametersProvider.GetParameters([]);

            Assert.Multiple(() =>
            {
                Assert.That(FileLog, Is.EqualTo(testFileLog));
                Assert.That(FileOutput, Is.EqualTo(testFileOutput));
                Assert.That(AddressStart, Is.EqualTo(testAddressStart));
                Assert.That(AddressMask, Is.EqualTo(testAddressMask));
                Assert.That(TimeStart, Is.EqualTo(testTimeStart));
                Assert.That(TimeEnd, Is.EqualTo(testTimeEnd));
            });
        }

        [Test]
        public void GetParameters_AllParametersInArgs_ReturnsAllParameters()
        {
            var args = new[]
            {
                "--file-log",  testFileLog,
                "--file-output", testFileOutput,
                "--address-start", testAddressStart,
                "--address-mask", testAddressMask,
                "--time-start", testTimeStart,
                "--time-end", testTimeEnd
            };

            var (FileLog, FileOutput, AddressStart, AddressMask, TimeStart, TimeEnd) = ParametersProvider.GetParameters(args);

            Assert.Multiple(() =>
            {
                Assert.That(FileLog, Is.EqualTo(testFileLog));
                Assert.That(FileOutput, Is.EqualTo(testFileOutput));
                Assert.That(AddressStart, Is.EqualTo(testAddressStart));
                Assert.That(AddressMask, Is.EqualTo(testAddressMask));
                Assert.That(TimeStart, Is.EqualTo(testTimeStart));
                Assert.That(TimeEnd, Is.EqualTo(testTimeEnd));
            });
        }

        [TestCaseSource(nameof(AbsentParameterInArgsCases))]
        public void GetParameters_AbsentParameter_ThrowsException(string[] args)
        {
            Assert.Throws<ApplicationException>(() =>
            {
                var (FileLog, FileOutput, AddressStart, AddressMask, TimeStart, TimeEnd) = ParametersProvider.GetParameters(args);
            });
        }

        public static object[] AbsentParameterInArgsCases =
        [
            new string[]
            {
                "--file-output", testFileOutput,
                "--address-start", testAddressStart,
                "--address-mask", testAddressMask,
                "--time-start", testTimeStart,
                "--time-end", testTimeEnd
            },
            new string[]
            {
                "--file-log",  testFileLog,
                "--address-start", testAddressStart,
                "--address-mask", testAddressMask,
                "--time-start", testTimeStart,
                "--time-end", testTimeEnd
            },
            new string[]
            {
                "--file-log",  testFileLog,
                "--file-output", testFileOutput,
                "--address-start", testAddressStart,
                "--address-mask", testAddressMask,
                "--time-end", testTimeEnd
            },
            new string[]
            {
                "--file-log",  testFileLog,
                "--file-output", testFileOutput,
                "--address-start", testAddressStart,
                "--address-mask", testAddressMask,
                "--time-start", testTimeStart,
            },
            new string[]
            {
                "--file-log",  testFileLog,
                "--file-output", testFileOutput,
                "--address-mask", testAddressMask,
                "--time-start", testTimeStart,
                "--time-end", testTimeEnd
            },
        ];
    }
}