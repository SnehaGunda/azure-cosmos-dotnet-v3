﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Azure.Data.Cosmos
{
    using System.Text.Json.Serialization;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// DOM for a composite path.
    /// A composite path is used in a composite index.
    /// For example if you want to run a query like "SELECT * FROM c ORDER BY c.age, c.height",
    /// then you need to add "/age" and "/height" as composite paths to your composite index.
    /// </summary>
    public sealed class CompositePath
    {
        /// <summary>
        /// Gets or sets the full path in a document used for composite indexing.
        /// We do not support wild cards in the path.
        /// </summary>
        [JsonPropertyName(Constants.Properties.Path)]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the sort order for the composite path.
        /// For example if you want to run the query "SELECT * FROM c ORDER BY c.age asc, c.height desc",
        /// then you need to make the order for "/age" "ascending" and the order for "/height" "descending".
        /// </summary>
        [JsonPropertyName(Constants.Properties.Order)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CompositePathSortOrder Order { get; set; }
    }
}