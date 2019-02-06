namespace TrafficComet.Splunk.LogWriter.Documents
{
	public class KeyValueDocument
	{
		public KeyValueDocument()
		{
		}

		public KeyValueDocument(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; set; }
		public string Value { get; set; }
	}
}
