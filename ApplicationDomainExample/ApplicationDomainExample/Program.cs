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
			// v1
			WriteLine("First example");
			WriteLine("---");
			V1();
			WriteLine("\n");

			// v2
			WriteLine("Second example");
			WriteLine("---");
			V2();
			WriteLine("\n");

			// v3
			WriteLine("Third example");
			WriteLine("---");
			V3();
			WriteLine("\n");

			WriteLine("The program has finished. Press any key to close...");
			ReadKey();
		}

		private static void V3()
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
		}

		private static void V2()
		{
			var otherAppDomain = AppDomain.CreateDomain("OtherAppDomain");

			var marshallByRefClassType = typeof(MarshallByRefClass);
			var serializableClassType = typeof(SerializableClass);

			var marshall = (MarshallByRefClass)otherAppDomain.CreateInstanceAndUnwrap(marshallByRefClassType.Assembly.FullName, marshallByRefClassType.FullName);
			var serializable = (SerializableClass)otherAppDomain.CreateInstanceAndUnwrap(serializableClassType.Assembly.FullName, serializableClassType.FullName);

			WriteLine(marshall.WhatIsMyAppDomain());
			WriteLine(serializable.WhatIsMyAppDomain());

			AppDomain.Unload(otherAppDomain);
		}

		private static void V1()
		{
			// other app domain
			var exampleAppDomain = AppDomain.CreateDomain("ExampleAppDomain");
			var thirdPartyType = typeof(ThirdPartyPackage);
			exampleAppDomain.CreateInstanceAndUnwrap(thirdPartyType.Assembly.FullName, thirdPartyType.FullName);

			AppDomain.Unload(exampleAppDomain);

			// current app domain
			var exampleClass = new ExampleClass();
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

	[Serializable]
	public class SerializableClass
	{
		public string WhatIsMyAppDomain()
		{
			return AppDomain.CurrentDomain.FriendlyName;
		}
	}

	public class MarshallByRefClass : MarshalByRefObject
	{
		public string WhatIsMyAppDomain()
		{
			return AppDomain.CurrentDomain.FriendlyName;
		}
	}
}
