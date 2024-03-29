using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="VersionsTest.CompareTest.g.cs"></copyright>
// <auto-generated>
// This file contains automatically generated tests.
// Do not modify this file manually.
// 
// If the contents of this file becomes outdated, you can delete it.
// For example, if it no longer compiles.
// </auto-generated>
using System;

namespace IntelliTestDemo.Tests
{
	public partial class VersionsTest
	{

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(NullReferenceException))]
public void CompareTestThrowsNullReferenceException48()
{
    int i;
    i = this.CompareTest((string)null, (string)null);
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException480()
{
    int i;
    i = this.CompareTest(".", "");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException654()
{
    int i;
    i = this.CompareTest(".\0", ".");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException333()
{
    int i;
    i = this.CompareTest(".\0", "\0");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException532()
{
    int i;
    i = this.CompareTest("", "");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException634()
{
    int i;
    i = this.CompareTest(":", "");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException666()
{
    int i;
    i = this.CompareTest(".\0", ".\0");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException700()
{
    int i;
    i = this.CompareTest(".", ".");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
public void CompareTest169()
{
    int i;
    i = this.CompareTest("4", "5");
    Assert.AreEqual<int>(-1, i);
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
public void CompareTest687()
{
    int i;
    i = this.CompareTest("1", "0");
    Assert.AreEqual<int>(1, i);
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException51()
{
    int i;
    i = this.CompareTest("0\0", "");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException130()
{
    int i;
    i = this.CompareTest("-0", "-");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException683()
{
    int i;
    i = this.CompareTest("0.", "0");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
[PexRaisedException(typeof(FormatException))]
public void CompareTestThrowsFormatException934()
{
    int i;
    i = this.CompareTest("\0.", "\0.");
}

[TestMethod]
[PexGeneratedBy(typeof(VersionsTest))]
public void CompareTest148()
{
    int i;
    i = this.CompareTest("8", "8.0");
    Assert.AreEqual<int>(0, i);
}
	}
}
