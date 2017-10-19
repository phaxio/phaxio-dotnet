using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Errors.V2;
using Phaxio.Tests.Helpers;
using System;

namespace Phaxio.Tests.UnitTests.V2
{
    [TestFixture]
    public class PhaxioClientTests
    {
        [Test]
        public void UnitTests_V2_NonJsonErrorHandled()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsText()
                .Content("There was an error")
                .InternalServerError()
                .Build<Response<AccountStatus>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            Assert.Throws(typeof(ServiceException), () => Console.Write(phaxio.Account.Status.Balance));
        }

        [Test]
        public void UnitTests_V2_RateLimitException()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/error_rate_limited"))
                .RateLimited()
                .Build<Response<AccountStatus>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            Assert.Throws(typeof(RateLimitException), () => Console.Write(phaxio.Account.Status.Balance));
        }

        [Test]
        public void UnitTests_V2_InvalidRequestException()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/error_invalid_entity"))
                .InvalidEntity()
                .Build<Response<AccountStatus>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            Assert.Throws(typeof(InvalidRequestException), () => Console.Write(phaxio.Account.Status.Balance));
        }

        [Test]
        public void UnitTests_V2_AuthenticationException()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/error_authentication"))
                .Unauthorized()
                .Build<Response<AccountStatus>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            Assert.Throws(typeof(AuthenticationException), () => Console.Write(phaxio.Account.Status.Balance));
        }

        [Test]
        public void UnitTests_V2_NotFoundException()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/error_not_found"))
                .NotFound()
                .Build<Response<AccountStatus>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            Assert.Throws(typeof(NotFoundException), () => Console.Write(phaxio.Account.Status.Balance));
        }

        [Test]
        public void UnitTests_V2_ServiceException()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/error_service"))
                .InternalServerError()
                .Build<Response<AccountStatus>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            Assert.Throws(typeof(ServiceException), () => Console.Write(phaxio.Account.Status.Balance));
        }
    }
}
