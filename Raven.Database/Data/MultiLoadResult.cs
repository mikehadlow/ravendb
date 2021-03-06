﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Raven.Database.Data
{
	/// <summary>
	/// Represent a result which include both document results and included documents
	/// </summary>
	public class MultiLoadResult
	{
		/// <summary>
		/// Gets or sets the document results.
		/// </summary>
		public List<JObject> Results { get; set; }
		/// <summary>
		/// Gets or sets the included documents
		/// </summary>
		public List<JObject> Includes { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiLoadResult"/> class.
		/// </summary>
		public MultiLoadResult()
		{
			Results = new List<JObject>();
			Includes = new List<JObject>();
		}
	}
}