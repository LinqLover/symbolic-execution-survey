using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelliTestDemo.Tests
{
    /// <summary>This class contains parameterized unit tests for Version</summary>
    [PexClass(typeof(Versions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class VersionsTest
    {
		/// <summary>Test stub for Compare(String, String)</summary>
		[PexMethod]
		public int CompareTest(string version1, string version2)
		{
			int result = Versions.Compare(version1, version2);
			return result;
			// TODO: add assertions to method VersionsTest.CompareTest(String, String)
		}

		[PexMethod]
		public int CompareTestWithOracle(string version1, string version2)
		{
			PexAssume.IsNotNullOrEmpty(version1);
			PexAssume.IsNotNullOrEmpty(version2);

			int result = 0;
			Exception exception = null;
			try
			{
				result = Versions.Compare(version1, version2);
			}
			catch (Exception _exception)
			{
				exception = _exception;
			}

			int otherResult = 0;
			Exception otherException = null;
			try
			{
				otherResult = new Version(version1).CompareTo(new Version(version2));
			}
			catch (Exception _exception)
			{
				otherException = _exception;
			}

			if (otherException == null)
			{
				PexAssert.IsNull(exception);
				PexAssert.AreEqual(otherResult, result);
			}
			else
			{
				PexAssert.AreEqual(otherException, exception);
			}

			return result;
		}
	}
}
