using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

			/* 
			 * The other example
			 * 
			AppDomain ad = AppDomain.CreateDomain("OtherAppDomain");

			var marshallByRefClassType = typeof(MarshallByRefClass);
			var serializableClassType = typeof(SerializableClass);

			MarshallByRefClass marshall = (MarshallByRefClass)ad.CreateInstanceAndUnwrap(marshallByRefClassType.Assembly.FullName, marshallByRefClassType.FullName);
			SerializableClass serializable = (SerializableClass)ad.CreateInstanceAndUnwrap(serializableClassType.Assembly.FullName, serializableClassType.FullName);

			Console.WriteLine(marshall.WhatIsMyAppDomain());
			Console.WriteLine(serializable.WhatIsMyAppDomain());*/

			ReadKey();
		}
	}

	// for current app domain
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
		// constructor
		public ThirdPartyPackage()
		{
			WriteLine("Third party package loaded");
			System.IO.File.Create($@"c:\temp\thirdparty-{Guid.NewGuid()}.log");
		}

		// destructor
		~ThirdPartyPackage()
		{
			WriteLine("Third party package unloaded");
		}
	}

	//[Serializable]
	//public class SerializableClass
	//{
	//	public string WhatIsMyAppDomain()
	//	{
	//		return AppDomain.CurrentDomain.FriendlyName;
	//	}
	//}

	//public class MarshallByRefClass : MarshalByRefObject
	//{
	//	public string WhatIsMyAppDomain()
	//	{
	//		return AppDomain.CurrentDomain.FriendlyName;
	//	}
	//}
}
