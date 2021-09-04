using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ApplicationDomainExample
{
	class Program
	{
		static void Main(string[] args)
		{

			// other app domain
			var thirdParty = new ThirdPartyPackage();
			var exampleAppDomain = AppDomain.CreateDomain("ExampleAppDomain");
			var thirdPartyType = typeof(ThirdPartyPackage);
			exampleAppDomain.CreateInstanceAndUnwrap(thirdPartyType.Assembly.FullName, thirdPartyType.FullName);

			// current app domain
			var exampleClass = new ExampleClass();
			ReadKey();
		}
	}

	class ExampleClass
	{
		public ExampleClass()
		{
			WriteLine("Now you are in current domain");
		}
	}

	// or use [Serializable] attribute
	class ThirdPartyPackage : MarshalByRefObject
	{
		// constractor
		public ThirdPartyPackage()
		{
			WriteLine("Third party package loaded");
			System.IO.File.Create($@"c:\temp\thirdparty-{Guid.NewGuid()}.log");
		}

		// destractor
		~ThirdPartyPackage()
		{
			WriteLine("Third party package unloaded");
		}
	}
}
