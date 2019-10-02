//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Azure.Data.Cosmos
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class CosmosResponseFactory
    {
        /// <summary>
        /// Cosmos JSON converter. This allows custom JSON parsers.
        /// </summary>
        private readonly CosmosSerializer cosmosSerializer;

        /// <summary>
        /// This is used for all meta data types
        /// </summary>
        private readonly CosmosSerializer propertiesSerializer;

        internal CosmosResponseFactory(
            CosmosSerializer defaultJsonSerializer,
            CosmosSerializer userJsonSerializer)
        {
            this.propertiesSerializer = defaultJsonSerializer;
            this.cosmosSerializer = userJsonSerializer;
        }

        internal Task<Response<T>> CreateItemResponseAsync<T>(
            Task<Response> cosmosResponseMessageTask,
            CancellationToken cancellationToken)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, async (cosmosResponseMessage) =>
            {
                T item = await CosmosResponseFactory.ToObjectInternalAsync<T>(cosmosResponseMessage, this.cosmosSerializer, cancellationToken);
                return new Response<T>(cosmosResponseMessage, item);
            });
        }

        internal async Task<T> ProcessMessageAsync<T>(Task<Response> cosmosResponseTask, Func<Response, Task<T>> createResponse)
        {
            using (Response message = await cosmosResponseTask)
            {
                return await createResponse(message);
            }
        }

        internal static ValueTask<T> ToObjectInternalAsync<T>(Response response, CosmosSerializer jsonSerializer, CancellationToken cancellationToken)
        {
            //Throw the exception
            // helper?
            if (response.Status < 200 || response.Status >= 300)
            {
                string message = $"Response status code does not indicate success: {response.Status} Reason: ({response.ReasonPhrase}).";

                throw new CosmosException(
                        response,
                        message);
            }
            if (response.ContentStream == null)
            {
                return new ValueTask<T>(Task.FromResult(default(T)));
            }

            return jsonSerializer.FromStreamAsync<T>(response.ContentStream, cancellationToken);
        }
    }
}