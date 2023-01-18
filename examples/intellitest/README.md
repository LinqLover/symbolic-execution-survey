# Example: Microsoft IntelliTest

## Installation

Install Visual Studio Enterprise (tested with 2017, newer versions should work as well):

<https://visualstudio.microsoft.com/de/downloads/>

## Usage

Open the solution in Visual Studio.

### Explore Method Behavior

Right-click [`Versions.CompareVersions()`](IntelliTestDemo/Versions.cs) and select <kbd>IntelliTest</kbd> → <kbd>Run IntelliTest</kbd>:

![IntelliTest Exploration Results](./assets/IntelliTest_explore+l.png#gh-light-mode-only)![IntelliTest Exploration Results](./assets/IntelliTest_explore+d.png#gh-dark-mode-only)

Another practical example of exploring method behavior is given in [`Helper.MatchRegex()`](IntelliTestDemo/Helper.cs) where IntelliTest can illustrate a regular expression by generating positive/negative input strings.

[`Helper.MatchHash()`](IntelliTestDemo/Helper.cs) and [`Program.Main()`](IntelliTestDemo/Program.cs) are two other examples that demonstrate some limitations of IntelliTest: exploration boundaries and overwhelming contingencies.

### Write Parametrized Unit Tests

Right-click [`Versions.CompareVersions()`](IntelliTestDemo/Versions.cs), select <kbd>IntelliTest</kbd> → <kbd>Create Unit Tests</kbd>, and confirm to add the tests to the existing test project.

This will generate a parametrized unit test method [`VersionsTests.CompareTest()`](IntelliTestDemo.Tests/VersionsTest.cs).

If you run these tests through the context menu, IntelliTest will generate a test for each possible combination of the input parameters (see [`VersionsTests.CompareTest.g.cs`](IntelliTestDemo.Tests/VersionsTest.CompareTestWithOracle.g.cs)).

You can add assumptions and assertions to the parametrized unit test methods as demonstrated in [`VersionsTests.CompareTestWithOracles()`](IntelliTestDemo.Tests/VersionsTest.cs).
