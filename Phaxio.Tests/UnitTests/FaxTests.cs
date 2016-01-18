using System;
using System.Linq;
using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using RestSharp;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using Phaxio.Entities.Internal;
using System.Collections.Generic;
using System.IO;

namespace Phaxio.Tests
{
    [TestFixture]
    public class FaxTests
    {
        [Test]
        public void UnitTests_Fax_SendSingleFileNoOptions()
        {
            var testToNumber = "8088675309";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, testToNumber);
            };

            var clientBuilder = new IRestClientBuilder { Op = "send", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var testFile = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.SendFax(testToNumber, testFile);

            Assert.AreEqual("1234", faxId, "FaxId should be the same.");
        }

        [Test]
        public void UnitTests_Fax_SendSingleFileOptions()
        {
            var testToNumber = "8088675309";
            var testOptions = new FaxOptions
            {
                HeaderText = "headertext",
                StringData = "somedata",
                StringDataType = "html",
                IsBatch = true,
                BatchDelaySeconds = 10,
                AvoidBatchCollision = true,
                CallbackUrl = "https://example.com/callback",
                CancelTimeoutAfter = 20,
                CallerId = "3213214321",
                FailureErrorType = "failure_type"
            };

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(testToNumber, parameters["to[]"]);

                var props = typeof(FaxOptions).GetProperties();
                foreach (var prop in props)
                {
                    var serializeAs = prop.GetCustomAttributes(false)
                        .OfType<SerializeAsAttribute>()
                        .FirstOrDefault();

                    if (serializeAs != null)
                    {
                        object expectedValue = prop.GetValue(testOptions, null);

                        Assert.AreEqual(expectedValue, parameters[serializeAs.Value]);
                    }   
                }
            };

            var clientBuilder = new IRestClientBuilder { Op = "send", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var testFile = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.SendFax(testToNumber, testFile, testOptions);

            Assert.AreEqual("1234", faxId, "FaxId should be the same.");
        }

        [Test]
        public void UnitTests_Fax_SendSingleFileSomeOptions()
        {
            var testToNumber = "8088675309";
            var testOptions = new FaxOptions
            {
                HeaderText = "headertext",
                StringData = "somedata",
                StringDataType = "html",
                IsBatch = true
            };

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(testToNumber, parameters["to[]"]);

                var props = typeof(FaxOptions).GetProperties();
                foreach (var prop in props)
                {
                    var serializeAs = prop.GetCustomAttributes(false)
                        .OfType<SerializeAsAttribute>()
                        .FirstOrDefault();

                    if (serializeAs != null)
                    {
                        object expectedValue = prop.GetValue(testOptions, null);

                        if (expectedValue == null)
                        {
                            Assert.False(parameters.ContainsKey(serializeAs.Value));
                        }
                        else
                        {
                            Assert.AreEqual(expectedValue, parameters[serializeAs.Value]);
                        }
                    }
                }
            };

            var clientBuilder = new IRestClientBuilder { Op = "send", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var testFile = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.SendFax(testToNumber, testFile, testOptions);

            Assert.AreEqual("1234", faxId, "FaxId should be the same.");
        }

        [Test]
        public void UnitTests_Fax_SendSingleWithTags()
        {
            var testToNumber = "8088675309";
            var testOptions = new FaxOptions
            {
                Tags = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } }
            };

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(testToNumber, parameters["to[]"]);

                foreach (var pair in testOptions.Tags)
                {
                    var tagKey = "tag[" + pair.Key + "]";
                    Assert.IsTrue(parameters.ContainsKey(tagKey));
                    Assert.AreEqual(pair.Value, parameters[tagKey]);
                }
            };

            var clientBuilder = new IRestClientBuilder { Op = "send", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var testFile = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.SendFax(testToNumber, testFile, testOptions);

            Assert.AreEqual("1234", faxId, "FaxId should be the same.");
        }

        [Test]
        public void UnitTests_Fax_SendMultipleFilesNoOptions()
        {
            var testToNumber = "8088675309";

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(testToNumber, parameters["to[]"]);

                Assert.AreEqual(3, parameters.Count());
            };

            var clientBuilder = new IRestClientBuilder { Op = "send", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var testFile = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.SendFax(testToNumber, new List<FileInfo> { testFile, testFile });

            Assert.AreEqual("1234", faxId, "FaxId should be the same.");
        }

        [Test]
        public void UnitTests_Fax_Cancel()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, "123");
            };

            var clientBuilder = new IRestClientBuilder { Op = "faxCancel", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.CancelFax("123");

            Assert.True(result.Success, "Should be success.");
        }

        [Test]
        public void UnitTests_Fax_Resend()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, "123");
            };

            var clientBuilder = new IRestClientBuilder { Op = "resendFax", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.ResendFax("123");

            Assert.True(result.Success, "Should be success.");
        }

        [Test]
        public void UnitTests_Fax_Delete()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, "123");
                Assert.AreEqual(req.Parameters[3].Value, false);
            };

            var clientBuilder = new IRestClientBuilder { Op = "deleteFax", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.DeleteFax("123");

            Assert.True(result.Success, "Should be success.");
        }

        [Test]
        public void UnitTests_Fax_DeleteWithOptions()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, "123");
                Assert.AreEqual(req.Parameters[3].Value, true);
            };

            var clientBuilder = new IRestClientBuilder { Op = "deleteFax", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.DeleteFax("123", true);

            Assert.True(result.Success, "Should be success.");
        }

        [Test]
        public void UnitTests_Fax_DownloadFax_NoOptions()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(parameters["id"], "1234");
            };

            var clientBuilder = new IRestClientBuilder { Op = "faxFile", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var testPdf = BinaryFixtures.getTestPdfFile();

            var pdfBytes = phaxio.DownloadFax("1234");

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }

        [Test]
        public void UnitTests_Fax_DownloadFax_WithOptions()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(parameters["id"], "1234");
                Assert.AreEqual(parameters["type"], "l");
            };

            var clientBuilder = new IRestClientBuilder { Op = "faxFile", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var testPdf = BinaryFixtures.getTestPdfFile();

            var pdfBytes = phaxio.DownloadFax("1234", "l");

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }
    }
}