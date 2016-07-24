using System;

namespace ChasePaymentech.Orbital.Test.Samples
{
	/// <summary>
	/// Summary description for IOrbitalSample.
	/// </summary>
	public interface IOrbitalSample 
	{
		void RunTest(int testCaseNumber);
		string Name
		{
			get;
		}
	}
}

