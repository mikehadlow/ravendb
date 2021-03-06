﻿using System.Collections.Generic;

namespace Raven.Bundles.Replication.Data
{
	/// <summary>
	/// This class represent the list of replication destinations for the server
	/// </summary>
    public class ReplicationDocument
    {
		/// <summary>
		/// Gets or sets the list of replication destinations.
		/// </summary>
        public List<ReplicationDestination> Destinations { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
        public string Id { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplicationDocument"/> class.
		/// </summary>
        public ReplicationDocument()
        {
            Id = "Raven/Replication/Destinations";
            Destinations = new List<ReplicationDestination>();
        }
    }
}