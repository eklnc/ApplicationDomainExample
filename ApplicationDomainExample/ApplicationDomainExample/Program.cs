using System;
using System.Security;
using System.Security.Permissions;

using static System.Console;

namespace ApplicationDomainExample
{
	class Program
	{
		static void Main(string[] args)
		{
			// permissions 
			var perm = new PermissionSet(PermissionState.None);
			perm.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			perm.AddPermission(new FileIOPermission(FileIOPermissionAccess.NoAccess, @"c:\"));

			var setup = new AppDomainSetup
			{
				ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
			};

			// other app domain which secured c:\
			var securedAppDomain = AppDomain.CreateDomain("SecuredAppDomain", null, setup, perm);
			//var thirdPartyType = typeof(ThirdPartyPackage);

			//// you will see constructor log but 
			//// you got System.Security.SecurityException now
			//securedAppDomain.CreateInstanceAndUnwrap(thirdPartyType.Assembly.FullName, thirdPartyType.FullName);

			try
			{
				var thirdPartyType = typeof(ThirdPartyPackage);


				// you got "System.Security.SecurityException" still but you unloaded the corresponding app domain now.
				securedAppDomain.CreateInstanceAndUnwrap(thirdPartyType.Assembly.FullName, thirdPartyType.FullName);
			}
			catch (Exception ex)
			{
				// maybe, you want to log the exception

				AppDomain.Unload(securedAppDomain);
			}

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
