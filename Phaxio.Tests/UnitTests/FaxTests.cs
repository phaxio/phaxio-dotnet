using System;
using System.Linq;
using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using Phaxio.Entities.Internal;
using System.Collections.Generic;
using System.IO;
using Phaxio.V2;

namespace Phaxio.Tests
{
    [TestFixture]
    public class FaxTests
    {
        [Test]
        public void UnitTests_V2_Fax_SendFax()
        {
            var testPdf = BinaryFixtures.getTestPdfFile();

            var request = new FaxRequest
            {
                ToNumber = "123",
                File = testPdf,
                ContentUrl = "http://example.com",
                HeaderText = "blah",
                BatchDelaySeconds = 10,
                AvoidBatchCollision = true,
                CallbackUrl = "http://example.org",
                CancelTimeoutAfter = 30,
                Tags = new Dictionary<string, string>() { { "key", "value" } },
                CallerId = "Bob",
                FailureErrorType = "Send failure"
            };

            Action<IRestRequest> parameterAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual("file[]", req.Files[0].Name);

                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(request.ToNumber, parameters["to[]"]);
                Assert.AreEqual(request.ContentUrl, parameters["content_url[]"]);
                Assert.AreEqual(request.HeaderText, parameters["header_text"]);
                Assert.AreEqual(request.BatchDelaySeconds, parameters["batch_delay"]);
                Assert.AreEqual(request.AvoidBatchCollision, parameters["batch_collision_avoidance"]);
                Assert.AreEqual(request.CallbackUrl, parameters["callback_url"]);
                Assert.AreEqual(request.CancelTimeoutAfter, parameters["cancel_timeout"]);
                Assert.AreEqual("value", parameters["tag[key]"]);
                Assert.AreEqual(request.CallerId, parameters["caller_id"]);
                Assert.AreEqual(request.FailureErrorType, parameters["test_fail"]);
            };

            var requestAsserts = new RequestAsserts()
                .Post()
                .Custom(parameterAsserts)
                .Resource("faxes")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/fax_send"))
                .Ok()
                .Build<Response<dynamic>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var id = phaxio.SendFax(request);

            Assert.AreEqual("1234", id);
        }

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
        public void UnitTests_V2_Fax_Cancel()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Post()
                .Resource("faxes/123456/cancel")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/fax_cancel"))
                .Ok()
                .Build<Response<Object>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var success = phaxio.CancelFax("123456").Success;

            Assert.IsTrue(success);
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
        public void UnitTests_V2_Fax_Resend()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Post()
                .Resource("faxes/123456/resend")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/fax_resend"))
                .Ok()
                .Build<Response<Object>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var result = phaxio.ResendFax("123456");

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
        public void UnitTests_V2_Fax_GetInfo()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("faxes/123456")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/fax_info"))
                .Ok()
                .Build<Response<FaxInfo>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var faxInfo = phaxio.GetFaxInfo("123456");

            DateTime createdAt = Convert.ToDateTime("2015-09-02T11:28:02.000-05:00");
            DateTime completedAt = Convert.ToDateTime("2015-09-02T11:28:54.000-05:00");

            Assert.AreEqual(123456, faxInfo.Id, "");
            Assert.AreEqual("sent", faxInfo.Direction, "");
            Assert.AreEqual(3, faxInfo.PageCount, "");
            Assert.AreEqual("success", faxInfo.Status, "");
            Assert.IsTrue(faxInfo.IsTest, "");
            Assert.AreEqual(completedAt, faxInfo.CompletedAt.Value, "");
            Assert.AreEqual(21, faxInfo.CostInCents, "");
            Assert.AreEqual("123", faxInfo.FromNumber, "");
            Assert.AreEqual("order_id", faxInfo.Tags.First().Key, "");
            Assert.AreEqual("1234", faxInfo.Tags.First().Value, "");
            Assert.AreEqual("321", faxInfo.ToNumber, "");
            Assert.AreEqual(42, faxInfo.ErrorId.Value, "");
            Assert.AreEqual("error_type", faxInfo.ErrorType, "");
            Assert.AreEqual("error_message", faxInfo.ErrorMessage, "");

            var recipient = faxInfo.Recipients.First();

            Assert.AreEqual("+14141234567", recipient.PhoneNumber);
            Assert.AreEqual(completedAt, recipient.CompletedAt.Value, "");
            Assert.AreEqual("success", recipient.Status, "");
            Assert.AreEqual(41, recipient.ErrorId.Value, "");
            Assert.AreEqual("recipient_error_type", recipient.ErrorType, "");
            Assert.AreEqual("recipient_error_message", recipient.ErrorMessage, "");
        }

        [Test]
        public void UnitTests_V2_Fax_Download()
        {
            var pdfInBytes = BinaryFixtures.GetTestPdf();

            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(parameters["thumbnail"], "l");
            };

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Custom(parameterAsserts)
                .Resource("faxes/123456/file")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsPdf()
                .RawBytes(pdfInBytes)
                .Ok()
                .Build();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var faxBytes = phaxio.DownloadFax("123456", thumbnail: "l");

            Assert.AreEqual(pdfInBytes, faxBytes, "Fax bytes should be the same");
        }

        [Test]
        public void UnitTests_V2_Fax_Delete()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Delete()
                .Resource("faxes/123456")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsPdf()
                .Content(JsonResponseFixtures.FromFile("V2/generic_success"))
                .Ok()
                .Build<Response<Object>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var result = phaxio.DeleteFax("123456");

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void UnitTests_V2_Fax_DeleteFiles()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Delete()
                .Resource("faxes/123456/file")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsPdf()
                .Content(JsonResponseFixtures.FromFile("V2/generic_success"))
                .Ok()
                .Build<Response<Object>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var result = phaxio.DeleteFaxFiles("123456");

            Assert.IsTrue(result.Success);
        }
    }
}