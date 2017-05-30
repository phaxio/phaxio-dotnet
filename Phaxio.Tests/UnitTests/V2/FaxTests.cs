using System;
using System.Linq;
using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Tests.Helpers;
using System.Collections.Generic;

namespace Phaxio.Tests.UnitTests.V2
{
    using Resources.V2;
    using Fax = Resources.V2.Fax;

    [TestFixture]
    public class FaxTests
    {
        [Test]
        public void UnitTests_V2_Fax_Create()
        {
            var testPdf = BinaryFixtures.getTestPdfFile();

            Action<IRestRequest> parameterAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual(testPdf.Name, req.Files.First().FileName);

                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual("123", parameters["to"]);
                Assert.AreEqual("http://example.com", parameters["content_url"]);
                Assert.AreEqual("blah", parameters["header_text"]);
                Assert.AreEqual(10, parameters["batch_delay"]);
                Assert.AreEqual(true, parameters["batch_collision_avoidance"]);
                Assert.AreEqual("http://example.org", parameters["callback_url"]);
                Assert.AreEqual(30, parameters["cancel_timeout"]);
                Assert.AreEqual("value", parameters["tag[key]"]);
                Assert.AreEqual("Bob", parameters["caller_id"]);
                Assert.AreEqual("Send failure", parameters["test_fail"]);
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
                .Build<Response<Fax>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var fax = phaxio.Fax.Create(to: "123",
                file: testPdf,
                contentUrl: "http://example.com",
                headerText: "blah",
                batchDelaySeconds: 10,
                avoidBatchCollision: true,
                callbackUrl: "http://example.org",
                cancelTimeoutAfter: 30,
                tags: new Dictionary<string, string>() { { "key", "value" } },
                callerId: "Bob",
                failureErrorType: "Send failure");

            Assert.AreEqual(1234, fax.Id);
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
                .Build<Response<object>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var fax = new Fax(123456);

            fax.PhaxioClient = phaxio;

            fax.Cancel();
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
                .Build<Response<object>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var fax = new Fax(123456);

            fax.PhaxioClient = phaxio;

            fax.Resend();
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
        public void UnitTests_V2_Fax_Retrieve()
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
                .Build<Response<Fax>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var faxInfo = phaxio.Fax.Retrieve(123456);

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

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var fax = new FaxFile(123456, thumbnail: "l");
            fax.PhaxioClient = phaxio;

            var faxBytes = fax.Bytes;

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
                .Build<Response<object>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var fax = new Fax(123456);

            fax.PhaxioClient = phaxio;

            fax.Delete();
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
                .Build<Response<object>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var file = new FaxFile(123456);

            file.PhaxioClient = phaxio;

            file.Delete();
        }
    }
}