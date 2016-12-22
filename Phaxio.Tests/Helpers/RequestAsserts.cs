using Phaxio.Entities.Internal;
using Phaxio.ThinRestClient;
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace Phaxio.Tests.Helpers
{
    class RequestAsserts
    {
        public const string TEST_KEY = "key";
        public const string TEST_SECRET = "secret";

        private Queue<Action<IRestRequest>> asserts = new Queue<Action<IRestRequest>>();

        public RequestAsserts Custom(Action<IRestRequest> assert)
        {
            asserts.Enqueue(assert);

            return this;
        }

        public RequestAsserts Auth()
        {
            asserts.Enqueue(request =>
            {
                var parameters = ParametersHelper.ToDictionary(request.Parameters);

                Assert.AreEqual((string)parameters[PhaxioConstants.KEY_NAME], TEST_KEY);
                Assert.AreEqual((string)parameters[PhaxioConstants.SECRET_NAME], TEST_SECRET);
            });

            return this;
        }

        public RequestAsserts Resource(string resource)
        {
            asserts.Enqueue(request =>
            {
                Assert.AreEqual(resource, request.Resource, "Resource was incorrect");
            });

            return this;
        }

        public RequestAsserts Get()
        {
            asserts.Enqueue(request =>
            {
                Assert.AreEqual(Method.GET, request.Method, "Method was incorrect");
            });

            return this;
        }

        public RequestAsserts Post()
        {
            asserts.Enqueue(request =>
            {
                Assert.AreEqual(Method.POST, request.Method, "Method was incorrect");
            });

            return this;
        }

        public RequestAsserts Delete()
        {
            asserts.Enqueue(request =>
            {
                Assert.AreEqual(Method.DELETE, request.Method, "Method was incorrect");
            });

            return this;
        }

        public Action<IRestRequest> Build()
        {
            return request =>
            {
                foreach (var assert in asserts)
                {
                    assert.Invoke(request);
                }
            };
        }
    }
}
