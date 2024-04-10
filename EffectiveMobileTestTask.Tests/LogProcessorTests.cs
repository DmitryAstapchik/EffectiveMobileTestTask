using System.Net;
using static EffectiveMobileTestTask.LogProcessor;

namespace EffectiveMobileTestTask.Tests
{
    [TestFixture]
    public class LogProcessorTests
    {
        private const string testLogFilePath = "testLogFile.txt";
        private const string testOutputFilePath = "testOutputFile.txt";
        private const string testTimeStart = "01.01.2024";
        private const string testTimeEnd = "31.12.2024";

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("NotExistentFile.txt")]
        public void ProcessLogFile_InvalidFileLog_ThrowsArgumentException(string? fileLog)
        {
            Assert.Throws<ArgumentException>(() => ProcessLogFile(fileLog!, testOutputFilePath, testTimeStart, testTimeEnd));
        }

        [Test]
        public void ProcessLogFile_MultipleIpVisitsInLogFile_ReturnsThemAll()
        {
            var logs = "0.0.0.0:2024-04-09 10:14:03\n" +
                "255.255.255.255 : 2024-05-10 11:15:04\n" +
                "255.255.255.255    :   2024-06-11 12:16:05\n" +
                "someInfo       255.255.255.255          :           2024-07-12 03:07:06    someOtherInfo";
            File.WriteAllText(testLogFilePath, logs);

            ProcessLogFile(testLogFilePath, testOutputFilePath, testTimeStart, testTimeEnd);

            var expected = "0.0.0.0 : 1\n" +
                "255.255.255.255 : 3";
            var actual = File.ReadAllText(testOutputFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ProcessLogFile_InvalidIpVisitsInLogFile_DoesNotThrowException()
        {
            var logs = "0.0.0.0 : invalidDate\n" +
               "invalidIp : 2024-05-10 11:15:04\n" +
               "someRandomString\n" +
               "255.255.255.255 :: 2024-07-12 13:17:06";
            File.WriteAllText(testLogFilePath, logs);

            Assert.DoesNotThrow(() =>
            {
                ProcessLogFile(testLogFilePath, testOutputFilePath, testTimeStart, testTimeEnd);
            });
            var actual = File.ReadAllText(testOutputFilePath);
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void ProcessLogFile_TimeStartAndTimeEnd_ReturnsCorrectData()
        {
            var logs = "0.0.0.0 : 2024-04-09 10:14:03\n" +
                "255.255.255.255 : 2024-05-10 11:15:04\n" +
                "255.255.255.255 : 2024-06-11 12:16:05\n" +
                "255.255.255.255 : 2024-07-12 13:17:06";
            File.WriteAllText(testLogFilePath, logs);

            ProcessLogFile(testLogFilePath, testOutputFilePath, "10.05.2024", "11.06.2024");

            var expected = "255.255.255.255 : 2";
            var actual = File.ReadAllText(testOutputFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ProcessLogFile_AddressStartAndAddressMask_ReturnsCorrectData()
        {
            var logs = "0.0.0.0 : 2024-04-09 10:14:03\n" +
                "255.255.255.255 : 2024-04-09 11:15:04\n" +
                "255.255.255.255 : 2024-06-11 12:16:05\n" +
                "255.255.255.255 : 2024-07-12 13:17:06\n" +
                "0.0.0.1 : 2024-04-09 10:14:03\n" +
                "0.0.0.1 : 2024-06-11 12:16:05\n" +
                "0.0.0.2 : 2024-04-09 10:14:03\n" +
                "0.0.0.2 : 2024-06-11 12:16:05";
            File.WriteAllText(testLogFilePath, logs);

            ProcessLogFile(testLogFilePath, testOutputFilePath, "09.04.2024", "09.04.2024", "0.0.0.1", "255.255.255.254");

            var expected = "0.0.0.1 : 1\n" +
                "0.0.0.2 : 1";
            var actual = File.ReadAllText(testOutputFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ProcessLogFile_AddressMaskWithoutAddressStart_ThrowsArgumentException()
        {
            var logs = "0.0.0.0 : 2024-04-09 10:14:03\n" +
                "255.255.255.255 : 2024-05-10 11:15:04\n" +
                "255.255.255.255 : 2024-06-11 12:16:05\n" +
                "255.255.255.255 : 2024-07-12 13:17:06";
            File.WriteAllText(testLogFilePath, logs);

            Assert.Throws<ArgumentException>(() =>
            {
                ProcessLogFile(testLogFilePath, testOutputFilePath, testTimeStart, testTimeEnd, null, "0.0.0.0");
            });
        }

        [Test]
        public void ProcessLogFile_AddressStartWithoutAddressMask_ReturnsCorrectRange()
        {
            var logs = "0.0.0.0 : 2024-04-09 10:14:03\n" +
                "1.1.1.1 : 2024-05-10 11:15:04\n" +
                "255.255.255.255 : 2024-06-11 12:16:05\n" +
                "255.255.255.255 : 2024-07-12 13:17:06";
            File.WriteAllText(testLogFilePath, logs);

            ProcessLogFile(testLogFilePath, testOutputFilePath, "01.01.2024", "01.07.2024", "1.1.1.1", null);

            var expected = "1.1.1.1 : 1\n" +
                "255.255.255.255 : 1";
            var actual = File.ReadAllText(testOutputFilePath);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}