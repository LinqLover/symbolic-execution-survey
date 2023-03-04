---
title: "Symbolic Execution and Applications"
author: [Christoph Thiede]
titlepage: true
abstract: |
  TODO: Abstract
mainfont: "Georgia"
sansfont: "Georgia"
filters:
- ./filters/gh_theme_context.py
- pandoc-fignos
biblatexoptions: [urldate=iso, date=iso, seconds=true]
header-includes: |
  \usepackage{float}
  \newfloat{lstfloat}{tbhp}{lop}
  \floatname{lstfloat}{Listing}
  \def\lstfloatautorefname{Listing}
# lstfloatautorefname might be needed for hyperref/auroref
...

# Introduction

*Program analysis* covers a wide range of techniques and programming tools for generating insights about the structure and the behavior of a software system.
We can distinguish between *static* and *dynamic* analysis tools:
static analysis tools examine a program by its source code to derive particular information (e.g., dependency analysis, type flow analysis) or detect occurrences of relevant patterns (e.g., spell checkers, linters).
Dynamic analysis tools, on the other hand, examine a program by observing the behavior of the running application (e.g., tests, benchmarks, or assertions).

While static analysis tools typically offer higher performance, they often provide a lower precision or selectivity than dynamic analysis.
For instance, static type flow analyzers are unable to reason about metaprogramming constructs such as Python's `getattr` function or JavaScript's `Function.prototype.bind()` method, and static performance prediction tools cannot respect bottlenecks of an individual hardware configuration.
However, while dynamic analysis tools typically offer a higher quality of results, they require *context* for running an application and reaching all its different states.
Programmers can provide context by composing a set of possible user interactions or by creating a comprehensive test suite, but this is a costly process and often, these artifacts do not yet exist or only cover a small portion of the code base.

*Symbolic execution* addresses this issue by automatically uncovering the execution paths of a program and performing analyses on them while requiring no or fewer specifications from programmers.
The fundamental operating principle of symbolic execution is to execute a program with *symbolic variables* instead of concrete values for its inputs and to fork the execution at each encountered conditional expression (e.g., an `if` statement) whose behavior depends on the concrete assignment of the symbolic variables.

In the last four decades, programmers and researchers have employed symbolic execution for different purposes such as vulnerability analysis of programs, program verification, unit testing, and various program understanding tasks including model generation and reverse engineering.

In this report, we give a summary of several implementations and applications of symbolic execution.
In \cref{approach}, we describe the fundamental operating principle of symbolic execution.
In \cref{technical-challenges}, we provide an overview of challenges for symbolic execution and existing solution approaches.
We present different use cases and tools for symbolic execution in \cref{applications} and examine their impact on research and industry in \cref{impact}.
In \cref{discussion}, we discuss limitations and usage considerations for different symbolic execution tools.
We classify some alternative approaches in \cref{related-tools} and finally give a conclusion in \cref{conclusion}.
Additional, we provide details on different implementation strategies (\cref{implementation-strategies}) and further resources (\cref{further-reading}) in the appendices.

# Approach

In this section, we describe the general approach of symbolic execution and introduce key concepts.

*Symbolic execution* (also abbreviated to *symbex* or *SymEx*) attempts to systematically uncover all execution paths of a program.
It takes a program in an executable form (e.g., x86 binary, LLVM bitcode, or JVM bytecode) and produces a list of inputs that trigger different code paths in the program.
Cadar describes it as a technique "to turn code 'inside out' so that instead of consuming inputs \[it\] becomes a generator of them" [@cadar2005execution].

To this end, the *symbolic execution engine* or the *symbolic executor* runs the program with its normal execution semantics but assigns a special *symbolic variable* to each input:

- As the program performs arithmetic or logic operations on symbolic variables, *symbolic expressions* are created that can be composed and stored in the *symbolic memory* (also *symbolic store*) of the program execution.
- As the program hits a branch instruction such as an `if` statement that depends on a symbolic condition, the executor decides the possible results of the depended expression (e.g., `true` or `false`).
  If the expression has multiple possible solutions, the entire execution is *forked* into multiple *execution paths* for each solution, and each fork is assigned a new *path constraint* for the assignment of its symbolic variable in the form of a Boolean first-order expression.

Together, the symbolic memory and the constraint set of an execution path constitute its *symbolic state* (see +@fig:foo-tree for the symbolic execution of the `foo()` function in \cref{lst:foo-code}).
The entirety of execution paths is named the *symbolic execution tree* of the program where each node represents a symbolic state and each edge represents a new path constraint.
For each leaf, i.e., each completed execution path, the symbolic execution engine can generate a concrete set of input values from the constraint, and programmers can use these concrete inputs to reproduce the same execution path in a regular non-symbolic context.

A crucial component of each symbolic execution engine is an *SMT solver* that decides whether a symbolic branch condition can be fulfilled and generates a concrete solution for a constraint set.
SMT solvers are a special kind of *SAT solvers* that test the *satisfiability* of a constraint set *modulo* ("within") a set of *theories*.
Theories are axiomatic systems for domain-specific algebras that enable the solver to reason not only about Boolean logic but also about predicates involving various datatypes.
Popular theories describe arithmetics, bitwise operations/integers with overflow semantics, or strings.

\begin{lstfloat}
  \begin{lstlisting}[language=c, label=lst:foo-code, caption={A simple function in C that can be executed symbolically.}]
void foo(int a, int b, int c) {
    int x = 0, y = 0, z = 0;
    if (a) {
        x = -2;
    }
    if (b < 0) {
        if (!a && c) {
            y = 1;
        }
        z = 2;
    }
}
  \end{lstlisting}
\end{lstfloat}

![The symbolic execution tree and concrete input sets for each path after analyzing the `foo()` method from \cref{lst:foo-code}. The symbolic variables $\alpha, \beta, \gamma$ represent the assigned values for the input variables `a`, `b`, and `c`.](./figures/symbex-tree.png){#fig:foo-tree width=65%}

<!-- For example, a symbolic execution of the `foo()` function in \cref{lst:foo-code} would produce the symbolic execution tree displayed in +@fig:foo-tree. -->

# Technical Challenges

<!-- In this section, we give an overview of common implementational challenges and solution strategies for symbolic execution.^[
  This section is by far the longest in this report, and an understanding of all the implementation details in \cref{taxonomy-of-executors} ff. is not relevant for readers who only came for the applications.
  However, as the implementation strategies illustrate the extent of the challenges, and as we further deem all of this super exciting stuff, we decided against moving them to an appendix.]

## Challenges -->

Despite the simple core concept of symbolic execution, several aspects of typical programs impede the exploration of all execution paths in a reasonable amount of time.
In this section, we give an overview of common implementational challenges and solution strategies.
\Cref{lst:challenges} gives examples for each challenge.
A more detailed description of modern implementation strategies is given in \hyperref[implementation-strategies]{appendix 1}.

\begin{lstfloat}
  \begin{lstlisting}[language=c, label=lst:challenges, caption={A selection of functions that illustrate common challenges of symbolic execution.}]
int count(int array[], int value) {
    int c = 0;
    // Path explosion!
    for (int i = 0; i < 100; i++) {
        if (array[i] == value) c++;
    }
    return c;
}

int product(int n) {
    int p = 1;
    // Infinite path explosion!
    for (int i = 1; i <= n; i++) {
        p *= i;
    }
    return p;
}

int read(char *fname) {
    // Blackbox!
    FILE *f = fopen(fname, "r");
    if (f == NULL) return -1;
    int x;
    // Blackbox!
    fscanf(f, "%d", &x);
    // Blackbox!
    fclose(f);
    return x;
}

int access(int index) {
    int array[10] = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
    // Symbolic pointer!
    if (array[index] == 5) return 1;
    return -1;
}

int check(char *password) {
    // Unsolvable constraint!
    if (superFastHash(password) == 0xdeadbeef) return 42;
    return 0;
}
  \end{lstlisting}
\end{lstfloat}

Path explosion, space explosion. \label{path-explosion}
:   For common programming constructs, the total number of execution paths may be superpolynomial in the number of conditional statements.
    For example, a program that checks a symbolic condition $n$ times within a loop or recursive function call can produce up to $2^n$ execution paths.
    If the break condition itself is symbolic, each check of the condition can generate additional execution paths, causing an infinite size of the execution tree.
    In reality, this results in an impractical performance even for symbolic execution of small programs [@cadar2013symbolic; @baldoni2018survey].
    
    Common solutions to this problem include restricting and prioritizing execution paths (\cref{path-selection}), static symbolic execution (\cref{static-symbolic-execution}) and function summarization (\cref{path-summarization}), and selective symbolic execution (\cref{selective-symbolic-execution}).

Environments, blackboxes.
:   Traditional symbolic execution assumes a whitebox program where the symbolic executor has access to all instructions.
    However, practical programs often interact with blackboxes, including system calls to the operating system (OS) or primitive calls to the virtual machine (VM) [@avgerinos2014enhancing; @baldoni2018survey].
    On the hardware level, all I/O operations are blackboxes, including accesses to file systems, hardware-accelerated computation, and network communication [@cadar2005execution; @baldoni2018survey].
    To symbolic executors that operate on the source code or intermediate representations of programs, also precompiled library binaries without sources represent a blackbox [@mirzaei2012testing].
    Importantly, event-driven applications that are built around a framework following the inversion of control principle [@johnson1988designing] can only be symbolically executed in the context of the framework implementation which is often complex (see \hyperref[path-explosion]{path explosion}) or unknown [@mirzaei2012testing].
    Finally, concurrent programs typically rely on a blackbox scheduler that is part of the OS or the VM [@yang2019advances, sec. 7].
    
    This issue is typically addressed using framework- or user-provided environment models or by lifting binaries to symbolically executable representations (see \cref{environment-models}).

Symbolic pointers.
:   If a pointer has a symbolic address value, naive symbolic execution engines cannot efficiently reason about the result of references to this pointer [@baldoni2018survey, sec. 3; @yang2019advances, sec. 6.3].
    In higher-level programming constructs, this problem corresponds to array or dictionary accesses using a symbolic index or key, or to method invocations on an object of a symbolic class or prototype [@baldoni2018survey, sec. 3].
    
    Advanced symbolic execution engines encounter this challenge by storing conditional expressions or symbolic pointers in the memory or by restricting the resolution of symbolic pointers while sacrificing completeness (\cref{memory-models}).

Solver limitations.
:   Constraint solving is an NP-complete problem for which no generic efficient solution algorithm is known [@cadar2013symbolic, sec. 4.2; @yang2019advances, sec. 3].
    In practice, this makes it hard to decide path conditions or generate inputs for constraints that contain nonlinear arithmetic expressions or one-way functions such as prime products or hashes [@banescu2016code].
    
    SMT solvers employ strategies such as parallelized and hardware-accelerated computing, optimized algorithms, caches, and other heuristics (\cref{constraint-solvers}).

# Applications

In the following, we present three common use cases of symbolic execution: program analysis, testing, and reverse engineering.

## Program Analysis

Through symbolic execution, programmers can automatically search large parts of their code bases for behavioral certain conditions or patterns.
For instance, they can find all statements that violate an invariant, or they can detect possibly unreachable code [@puasuareanu2009survey, sec. 5].
It is also possible to infer new invariants from the execution tree [@csallner2008dysy].

A highly popular application of symbolic execution is *vulnerability detection* that allows programmers to scan programs for edge cases and circumstances causing security violations.
In this context, several definitions of security violations can be used:
in the simplest form, symbolic execution tools can uncover all paths that lead to an irregular abort or crash of a program [@cadar2008klee; @godefroid2008automated; @cha2012unleashing].
In advanced scenarios, tools install an agent that tries to exploit potential attack vectors such as remote code execution, or they even detect suspicious behavior patterns such as accesses to uninitialized memory [@avgerinos2014automatic].
Some tools perform *exploit generation* by creating programs or sets of input values that programmers can use to reproduce the detected vulnerabilities (see +@fig:klee-exploits) [@cadar2008klee; @cha2012unleashing; @avgerinos2014automatic].

![Original exploits in the GNU Core Utils library that cause program crashes, generated by the symbolic execution engine [KLEE]{.smallcaps} [@cadar2008klee].](./figures/klee-exploits.png){#fig:klee-exploits width=50%}

## Testing

Analogously to exploit generation, tools can also employ symbolic execution to *generate inputs* or *crash tests* for all covered execution paths of a program [@crosshair; @albert2014test].
Many tools generate code that contains only the arrange and act logic of a test but leave it to programmers to add assertions [@crosshair]; however, some approaches incorporate infered invariants to generate assertions [@csallner2008dysy; @nguyen2017symlnfer].

*Symbolic testing frameworks* go beyond unspecific test generation by allowing programmers to write tests that operate on symbolic variables rather than concrete values [@tillmann2008pex; @goodman2018deepstate] (see \cref{lst:pexmethod}).
Instead of specifying actual example inputs and expected outputs, programmers declare preconditions (*assumptions*) on symbolic variables and postconditions (assertions) on the results from the system under test.
The testing framework executes these tests symbolically and reports any inputs or input constraints that violate the symbolic assertions.
Programmers can also use symbolic tests to *compare different implementations*, e.g., for detecting breaking changes in a interface [@schanely2022contractual].

\begin{lstfloat}
  \begin{lstlisting}[language={[Sharp]C}, label=lst:pexmethod, caption={A symbolic unit test written in the parametrized unit testing framework \textsc{Pex} for C\# \autocite{tillmann2008pex}. The test asserts that for each pair of non-empty version strings, the method \passthrough{\lstinline!Versions.Compare()!} returns either \passthrough{\lstinline!-1!}, \passthrough{\lstinline!0!}, or \passthrough{\lstinline!1!}.}]
[PexMethod]
public int CompareTest(string version1, string version2)
{
    PexAssume.IsNotNullOrEmpty(version1);
    PexAssume.IsNotNullOrEmpty(version2);
    
    int result = Versions.Compare(version1, version2);
    
    PexAssert.IsTrue(new[] { -1, 0, 1 }.Contains(result));
    
    return result;
}
  \end{lstlisting}
\end{lstfloat}
<!-- TODO: Fix floating (top or bottom!) -->

A similar style of testing is *contract programming* where programmers specify pre- and postconditions for individual functions or methods that can be validated by a symbolic execution engine (see +@fig:crosshair-contract) [@crosshair].
Other than testing frameworks, contracts are typically attached directly to the code under test, and many frameworks feature declarative specifications that are less suitable for imperative arrangements or disjoint invariants.

![Screenshot of a method contract expressed as a Python docstring in Visual Studio Code. The symbolic execution tool [CrossHair]{.smallcaps} reports live feedback on the contract after saving the file and displays a violation of the specified postcondition [@crosshair].](./figures/crosshair_contract+l.png#gh-light-mode-only){#fig:crosshair-contract}![Screenshot of a method contract expressed as a Python docstring in Visual Studio Code. The symbolic execution tool [CrossHair]{.smallcaps} reports live feedback on the contract after saving the file and displays a violation of the specified postcondition [@crosshair].](./figures/crosshair_contract+d.png#gh-dark-mode-only){#fig:crosshair-contract}

Symbolic testing tools can support programmers in improving the code coverage of their systems and provide them with faster feedback to discover bugs while reducing the required cost for writing tests.
However, programmers take the usual overhead of writing tests as they need to provide fakes, mocks, and stubs for external units.
The need for formal pre- and postconditions further increases this overhead:
typically, systems are based on many implicit assumptions, and programmers are forced to explicate them when deciding for the edge cases that their application should handle.
If programmers have to deal with generated tests, typical concerns of code generation may arise, including readability and maintainability of generated code and concretized values (see \cref{constraint-solvers}).
Finally, performance and theories of symbolic executors are limited, causing long wait times and incomplete coverage for complex programs (see \cref{technical-challenges}).

## Reverse Engineering

Another category of symbolic execution tools support programmers in *reverse engineering* tasks by allowing them to explore the captured behavior of executed units through alternative representations.

Microsoft [IntelliTest]{.smallcaps} summarizes the behavior of a method by constructing a table of input arguments, return values, and optionally user-specified expressions from the symbolic execution paths (see +@fig:intellitest) [@intellitest; @tillmann2008pex].
Programmers can benefit from this to understand methods and explore their edge cases without studying the implementation.

![Screenshot of the [IntelliTest]{.smallcaps} explorer in Microsoft Visual Studio summarizing the behavior of the method `Versions.Compare()` [@intellitest]. Each result represents a different equivalence class of inputs (arguments) and outputs (return value or raised exception) that are caused by the same code path. On the right, the test code for reproducing the example (generated using [Pex]{.smallcaps}, see \cref{testing}) is shown.](./figures/IntelliTest_explore+l.png#gh-light-mode-only){#fig:intellitest}![Screenshot of the [IntelliTest]{.smallcaps} explorer in Microsoft Visual Studio summarizing the behavior of the method `Versions.Compare()` [@intellitest]. Each result represents a different equivalence class of inputs (arguments) and outputs (return value or raised exception) that are caused by the same code path. On the right, the test code for reproducing the example (generated using [Pex]{.smallcaps}, see \cref{testing}) is shown.](./figures/IntelliTest_explore+d.png#gh-dark-mode-only){#fig:intellitest}

Other tools use symbolic execution to aid program understanding through *symbolic debugging*:
programmers can debug a program without providing concrete entrypoints (see +@fig:sed) [@hentschel2019symbolic; @sed; @ponce].
All variables are initialized symbolically, and during debugging, programmers can interactively advance and explore a symbolic execution tree of the selected method.
This style of debugging may help programmers to evade irrelevant or distracting context, but they also may experience symbolic expressions in the memory of the program as too abstract and sophisticated.
The extent of the symbolic execution tree may be overwhelming, and its structure may be confusing as it mismatches common control flow patterns such as loops.
Thus, the value for symbolic debugging tools is likely maximal for codebases with a high complexity and a poor readability; for instance, several disassembling tools support symbolic debugging [@ponce].

![Screenshot of the *Symbolic Execution Debugger* ([SED]{.smallcaps}) while exploring the execution tree of a `QuickSort` implementation for Java in Eclipse [KLEE]{.smallcaps} [@hentschel2019symbolic; @sed]. By selecting a node in the tree, programmers can inspect its symbolic state in the remaining panes and advance the execution of the code path stepwise.](./figures/SED.png){#fig:sed}

Like symbolic testing tools, symbolic reverse engineering tools suffer from the limited performance and coverage of execution engines, and programmers are required to specify additional context to explore code that interacts with an external environment or is based on implicit assumptions.

# Impact

In this section, we review the adoption of symbolic execution and its impact on particular software systems.

## Adoption of Symbolic Execution Tools

As an example, we evaluate the adoption of symbolic execution tools in the open source community by considering a selected sample of repositories on GitHub that use symbolic execution concepts, are actively maintained, and have an above-average popularity (at least 200 stars).
The largest group of symbolic execution tools based on this criteria are frameworks and libraries that enable programmers to perform a dynamic analysis of their systems on a low or medium abstraction level.
For instance, many tools offer APIs for running particular instructions or methods symbolically and inspecting the resulting execution tree [@shoshitaishvili2016sok; @desclaux2012miasm; @saudel2015triton].
A large number of tools provides means for semi-automated vulnerability detection and exploit generation [@mythril; @onefuzz].
Most tools operate on binaries; however, some solutions target applications for Java, JavaScript, or smart contracts that are run in the Ethereum VM [@mossberg2019manticore; @mythril].
Some tools also use symbolic execution to detect suspicious behavioral patterns such as the propagation of null pointers or object layouts and accesses that impede particular JIT optimizations [@jalangi2], to verify automated optimizations [@alive2], or to assist programmers with the exploration of disassemblied code [@ponce].

To our knowledge, few solutions are widespread that assist programmers with writing tests [@crosshair; @goodman2018deepstate].
An exception is IntelliTest which has been integrated in Microsoft Visual Studio since 2015 and thus is available to a potentially large number of users [@intellitest].^[
  IntelliTest is available in the Enterprise edition of Visual Studio.
  More than 150,000 companies have subscribed to Visual Studio [@enlyft], and Microsoft provides free access to the Visual Studio Enterprise for students through the *Azure Education Hub* program [@educationhub].
  However, the actual number of customers who work with C#/.NET Framework is unknown.
]

## Impact for Vulnerability Detection

In the past two decades, symbolic execution tools have had a particular impact regarding detecting vulnerabilities (see \cref{program-analysis}).
Microsoft has used their in-house concolic execution engine [SAGE]{.smallcaps} to find more than 30% of all bugs in Windows 7 that were fixed prior to the release, has later established the tool as a standard component of their internal testing pipelines, and is continuously running it 24/7 on more than 200 machines to reduce the number of exploits in several products [@godefroid2012sage; @bounimova2013billions].

In research, the popular free open-source library GNU Core Utils (containing tools such as `cat`, `tee`, and `wc`) has been established as a benchmark for evaluating the performance of symbolic execution engines.
The entire library comprises 89 binaries with 72 kLOC (thousands lines of code) and the original test suite had a coverage of 67.6% LCOV (percent of lines covered) [@cadar2008klee].
In 2008, [KLEE]{.smallcaps} detected 56 yet unknown bugs and crashes in the library within 1 hour per binary and increased the overall code coverage to 84.5% (see \cref{fig:klee-exploits}) [@cadar2008klee].
Later, other tools detected further bugs in the library while reducing the operational costs [@marinescu2012make; @kuznetsov2012efficient].
In 2012, [Mayhem]{.smallcaps} reached a coverage of 97.6% LCOV for a subset of the library within 1 hour per binary [@cha2012unleashing].

Researchers have also demonstrated the ability of vulnerability detection for other software systems:
Inter alia, bugs and vulnerabilities were found in BusyBox [@cadar2008klee], the Linux kernel [@cha2012unleashing], Minix [@cadar2008klee], and Windows [@cha2012unleashing].
[MergePoint]{.smallcaps} reached particularly remarkable results by discovering more than 11,000 bugs in the entire Debian kernel [@avgerinos2014enhancing] (consisting of more than 33,000 binaries and 679 MSLOC (millions of source lines of code) [@debian]).
This analysis took them 18 CPU-months but benefited from massive parallelization across multiple machines.
They also estimated that the operational costs for renting virtual servers in a data center would correspond to $0.28 per discovered bug, showing that symbolic execution for security analysis can be financially worthwhile given the potentially high costs of zero-day exploits that are discovered in the public [@godefroid2012sage; @castillo2016dao; @perlroth2021untold].

# Discussion

Symbolic execution has proven as useful for several applications such as bug detection, test generation, or program exploration.
Programmers can improve the test coverage of their systems and gain faster insights in the behavior and edge cases of programs while reducing the effort and distractions of considering context.
Still, symbolic execution imposes several limitations and cost factors:
depending on the complexity of an application, performance may be insuitable for interactive response times or even require hours to days and considerable financial resources.
The coverage of execution paths is limited and depends on the coupling of a software to blackboxes and environments, algebraic complexity of algorithms, and usage of lookup constructs such as pointers or large dictionaries.
In many cases, programmers are required to provide additional configuration to model blackboxes or specify some context, and generated artifact may suffer from poor readability or sustainability.

Thus, programmers need to evaluate the suitability of symbolic execution tools for particular software projects by weighting these costs against the potential value of tools.
For instance, for safety- or security-critical applications that involve domains such as finances or user data, programmers might prioritize high quality and dependability of their software over a fast and inexpensive development process.
The value of tools that aim to improve the programming experience and aid program understanding mainly correlates to the implementational complexity of projects.

# Related Tools

Next to symbolic execution, programmers may also consider alternative approaches and tools.
*Static analysis tools* [@gosain2015static; @emanuelsson2017comparative; @li2018survey] and *fuzzing tools* [@zeller2019fuzzing; @garus2023fuzzing] typically provide significantly better performance but deliver lower precision and selectivity as they cannot systematically scan all execution paths.
Still, these techniques are more popular and, for many languages and environments, may surpass symbolic execution in terms of quantity and maturity of available solutions.
*Generative AI software* such as [GitHub Copilot]{.smallcaps} or [ChatGPT]{.smallcaps} forms another category of tools that can practically assist programmers with analyzing programs or generating tests, but their results occasionaly suffer from poor and unreliable quality especially for unpopular domains, and programmers often experience the workflow as awkward and unsatisfying [@barke2022grounded; @vaithilingam2022expectation; @sobania2023analysis].

# Conclusion

Symbolic execution is a dynamic program analyis technique which systematically identifies and examines the most execution paths of a program without requiring programmers to provide concrete inputs.
It is used in several types of programming tools to assist programmers with tasks such as bug detection, exploit generation, testing, and program comprehension and has been successfully adopted in the industry for both internal purposes and commercial products.
Still, performance and completeness of symbolic execution are limited, and programmers face a possible overhead for specifying required context about the environment of the program.

\appendix

# Implementation Strategies

In this appendix, we describe several modern implementation strategies and considerations that address existing challenges (\cref{technical-challenges}) and optimize symbolic execution in more detail.

## Taxonomy of Executors

Symbolic executors can follow alternative design decisions that make different trade-offs between implementation costs, runtime performance, and systemic limitations.

### Online and Offline Symbolic Execution

Traditional symbolic execution as described in \cref{approach} is classified as *online symbolic execution* where every covered execution path is executed exactly once and the symbolic state is *cloned* at each symbolic branch condition.
On the other hand, *offline symbolic executors* do not fork the execution but instead re-run an execution path for each selected branch [@baldoni2018survey].

While online symbolic execution processes fewer instructions in total, it maintains many cloned states in the memory which can be infeasible or require frequent swapping to slower storage [@baldoni2018survey].
However, online symbolic executors can reduce memory consumption by using copy-on-write data structures to share common symbolic states among multiple execution paths [@baldoni2018survey].
Offline symbolic execution does not share any resources between multiple execution paths, reducing the implementation effort for resource isolation (see also \cref{environment-models}).

*Hybrid symbolic executors* combine both online and offline execution styles to benefit from the improved performance of online execution until a memory boundary is reached [@cha2012unleashing].
At this point, they switch to an offline execution strategy and re-execute further execution paths from the beginning.

### Dynamic Symbolic Execution

Traditional symbolic execution is unable to handle blackboxes:
as any variables are possibly assigned symbolic expressions, the executor cannot pass them to a foreign system (e.g., when making a system call to the OS).
*Dynamic symbolic execution* (DSE) resolves this limitation by combining symbolic execution with normal concrete execution which assigns concrete values to variables [@cadar2013symbolic; @baldoni2018survey].^[
  The terms "dynamic symbolic execution" and "concolic execution" are used inconsistently in the literature.
  In this report, we adhere to the taxonomy of Cadar et al. who use "dynamic symbolic execution" as an umbrella term for execution-generated testing and concolic execution [@cadar2013symbolic] (instead of the taxonomy of Baldoni et al. who use both terms in the opposite way [@baldoni2018survey]).]

One form of DSE is *execution-generated testing* (EGT) which interleaves symbolic and concrete execution\ [@cadar2005execution].
When a blackbox is called, the executor concretizes any symbolic arguments before passing them to the blackbox by requesting a solution for the current constraint set from the SMT solver.

*Concolic execution* (a portmanteau of "concrete" and "symbolic") is another form of DSE which executes a program both in concrete and symbolic style simultaneously [@sen2005cute; @godefroid2008automated].
Concrete execution maintains a concrete state of all variables and is used to direct the control flow and call blackboxes.
Symbolic execution, accompanies concrete execution to collect the symbolic constraints for the taken conditional branches.
Once an execution path has been completed, further paths can be generated by negating single constraints from the collection and generating concrete input values for the next execution.

Thus, concolic execution shifts the usage patterns of the SMT solver [@baldoni2018survey, sec. 2]:
the solver is only requested once per execution path to generate constraints, removing the context-switch overhead of requesting the solver at every conditional instruction.
If the solver is unable to find a solution for a constraint set, symbolic execution can omit some execution paths instead of inevitably aborting [@cadar2013symbolic, sec. 3].
Practically, concolic execution is implemented by instrumenting the program for collecting constraints and running the instrumented program offline in the regular OS or VM [@sen2007concolic; @poeplau2020symbolic].
This also improves performance and reduces implementation costs as a level of indirection for a separate interpreter is avoided.

Still, neither EGT nor concolic execution reveals conditional branches inside blackboxes.
For improving the coverage of blackboxes, programmers can alternatively specify memory models (\cref{memory-models}).

### Static Symbolic Execution

*Static symbolic execution* (SSE) tackles the issue of path explosion by deriving a static transformation of program parts rather than executing them [@avgerinos2014enhancing; @baldoni2018survey].
SSE employs *function summary* and *loop summary* techniques to translate a function or a group of instructions, resp., to an equivalent conditional symbolic expression (corresponding to a mathematical piecewise-defined function).
For instance, *compositional symbolic execution* analyzes single program parts in isolation by treating any function calls as new symbolic expressions and translates them to pairs of preconditions and postconditions [@yang2019advances, sec. 5].
SSE reduces the cost for executing a program multiple times and deciding/negating constraint sets but increases the pressure on the SMT solver to handle conditional expressions; however, recent advances in solvers make this a worthwhile shift (see also \cref{constraint-solvers}) [@avgerinos2014enhancing].
Two limitations of SSE are that it cannot handle blackboxes and that it cannot follow any control flow patterns beyond basic conditional or loop jumps [@avgerinos2014enhancing].

*Veritesting* is another technique that combines the strengths of SSE and concolic execution:
it runs a program in SSE mode as long as possible and falls back to concolic execution when a function boundary, blackbox, or another limitation of SSE is hit.
From concolic mode, veritesting returns to SSE mode at the next supported instruction.
Veritesting outperforms regular DSE by more than 70% [@avgerinos2014enhancing].

### Backward Symbolic Execution

Another approach to symbolic execution is *backward symbolic execution* (BSE) which explores instructions and execution paths of a program in reverse order [@ma2011directed; @baldoni2018survey, sec. 2.3].
One application of BSE is post-mortem debugging [@chen2014star].
To identify the possible callers of a method, a statically generated control-flow graph (CFG) can be used.
As methods might have multiple callers, BSE is also affected by the path explosion problem [@baldoni2018survey, sec. 2.3].

<!-- TODO: BSE is not very mature -- maybe remove or communicate brevity? -->

## Selective Symbolic Execution

Complete symbolic execution of a software system can take hours up to months [@avgerinos2014enhancing], but in many cases, programmers are only interested in the results of analyzing particular subsets or behaviors of the system.
*Selective symbolic execution* includes several methods for selecting parts of the system for analysis [@chipounov2011s2e].
In its general form, programmers can specify an allow- or denylist of units (e.g., modules, classes, or methods) to be analyzed.
All non-selected parts are treated as blackboxes: outside of the selected parts, the program can be executed concretely without generating new execution paths.
However, this approach reduces the completeness of the analysis; for instance, not all possible return values or side effects from a call to a skipped function will be covered.
*Chopped symbolic execution* can partially restore the lost completeness by employing static analysis of the skipped function's behavior\ [@trabish2018chopped].

Selective symbolic execution is well combinable with compositional symbolic execution where single units can be analyzed in isolation.

*Shadow symbolic execution* is another form of selective symbolic execution that selects parts for symbolic execution based on the changes to a software system against the latest run of the symbolic execution engine.
In the context of continuous integration where tests and analyses are run for each new revision, this can significantly reduce the execution time or improve the coverage within a given time limit, resp. [@kuchta2018shadow]

Using *preconditioned symbolic execution*, programmers can manually specify restrictions on inputs or program behavior [@avgerinos2014automatic; @baldoni2018survey, sec. 5.5].
For instance, they can limit the size of certain buffers to 100 bytes, disallow non-ASCII characters, or provide regular grammars for generated inputs.

Instead of binary filters, programmers can also specify a priority list of different program parts.
In *directed symbolic execution* or *shortest-distance symbolic execution*, the selected program parts are analyzed first, followed by adjacent program parts based on their distance from the selected regarding the static CFG of the system [@ma2011directed].
Similarly, *lazy expansion* explores the call graph of a test method top-down by initially treating all function calls as blackboxes and descending into them later [@majumdar2007latest].

## Path Management

A major challenge of symbolic execution is path explosion where the number of execution paths may grow exponentially to the number of conditional branches or even infinitely.
Different strategies exist to cope with the large size of the execution tree, and practical symbolic execution engines commonly combine multiple of them.

### Path Selection

*Path selection* (also *branch prioritization* or *search strategies*) refers to the idea of maximizing the number of *relevant* execution paths within a given time limit [@liu2017survey; @baldoni2018survey, sec. 2.2].
A criterion is defined for ordering execution paths, and the offline executor selects the next path based on this criterion.

Naive path selection strategies include *depth-first search* (DFS) and *breadth-first search* (BFS) of the execution tree [@liu2017survey; @sabbaghi2020systematic].
DFS is a prime example of search strategies that are not loop-safe:
if the executor hits an infinite subtree (e.g., a loop or recursive call with a symbolic break condition), the search will never complete, and the remaining tree will not be explored further.
BFS, on the other hand, involves a maximum memory overhead for preserving incompletely explored parts of the execution tree and is unlikely to explore relevant deeply nested subtrees early [@liu2017survey].
*Random search* combines the strengths of DFS and BFS by randomly selecting execution paths but is still not loop-safe [@liu2017survey].

Other strategies assign scores or weights to different execution paths.
*Generational search* maximizes execution order for code coverage by computing the score of each new from the relative coverage increase of the previous (generating) path [@godefroid2008automated; @sabbaghi2020systematic, p. 24f.].
Execution paths can also be prioritized based on the mutation coverage of the generated tests or the coverage of the static CFG [@sabbaghi2020systematic, p. 18ff.].
Some strategies use heuristics to predict the number of errors in subtrees based on certain instruction types or the historic density of defects in a unit (assuming that the defect density is not normally distributed amongst units but peaks in units written by different authors on different days) [@cha2012unleashing; @avgerinos2014automatic; @sabbaghi2020systematic, p. 26].
Further heuristics try to detect repetitive loop iterations or recursive calls or use define *fitness functions* for symbolic variables to prioritize solutions for them that trigger certain branches [@cadar2013symbolic, p. 9; @sabbaghi2020systematic, p. 27].

### Path Summarization

*Path summarization* strategies analyze single functions or loops statically to avoid path explosion.
These strategies commonly overlap with techniques employed for SSE (\cref{static-symbolic-execution}).

### Path Pruning and Merging

*Path pruning* or *path subsumption* strategies detect identical execution paths and eliminate doubles [@baldoni2018survey, sec. 5.4; @yang2019advances, sec. 4.2].
By storing pre- and postconditions, they can also detect *equivalent* execution paths that only differ in side effects or values that are irrelevant to the remaining control flow.

*Path merging* strategies avoid path multiplicities by combining similar execution paths [@kuznetsov2012efficient; @avgerinos2014enhancing; @baldoni2018survey, sec. 5.6; @yang2019advances, sec. 4.3].
The state of merged execution paths contains conditional constraints and conditional expressions in the symbolic store.
To weigh up the reduced costs for execution against the additional pressure on the SMT solver, they employ several heuristics.
Among others, these heuristics take into account the following instruction types and the static CFG of a program to predict the solver costs.

### Parallelization

Depending on the architecture of the software under investigation, the available input seeds (e.g., from a test suite), and the resulting shape of the execution trees, symbolic execution can be accelerated by distributing the execution tree to multiple processors or multiple machines [@bucur2011parallel; @avgerinos2014enhancing].

## Environment Models

To avoid blackboxes, programmers can provide models to emulate parts of the environment [@baldoni2018survey, sec. 4].
Similar to unit testing, they can configure fakes and stubs that are whiteboxes to the symbolic executor\ [@deHalleux2010moles].
For instance, they can replace the real filesystem with a virtual filesystem or redirect all requests to a remote server to a stub.
Fakes can model uncertainties about the original environment by creating new symbolic variables, e.g., to represent the contents of a file or to decide whether a server is reachable.
For large blackboxes, some tools approach to automate model generation.

For event-driven applications that pass control to a framework, such as a GUI framework for web pages or mobile applications, another approach is the compositional analysis of single call targets that are exposed to the framework [@mirzaei2012testing].
These *sub-call graphs* can then be composed based on regular grammars that describe possible call sequences.

Other approaches attempt to disclose blackboxes by lifting binary code to a higher-level representation such as LLVM bitcode that is compatible with the symbolic executor [@cadar2008klee] or by employing virtualization techniques to emulate critical instructions [@chipounov2011s2e].

## Memory Models

To properly handle symbolic pointers, symbolic execution engines have to consider all possible symbolic states that can result from dereferencing the pointer (or looking up an array element, resp.) [@cadar2013symbolic; @baldoni2018survey, sec. 3].
As this is prone to path explosion, one alternative is to store a conditional expression in the symbolic memory that represents all possible results and can be handled from a single execution path (similar to path merging, see \cref{path-pruning-and-merging}).
For write operations, it is also possible to use the unresolved symbolic address as a key in the symbolic store.
Still, for read operations, this approach would frequently lead to an explosion of the symbolic store, given the typical address space of programs.

Other approaches set boundaries to the address space [@baldoni2018survey, sec. 3].
For instance, symbolic execution engines can constraint symbolic pointers to all previously allocated addresses plus a canonical null pointer, or they may immediately report an error for an execution path that accesses unallocated space as this is deemed an unsafe practice.
Alternatively, offline engines can randomly concretize symbolic pointers, allocate new space dynamically, and (optionally lazily [@yang2019advances, sec. 6]) initialize it with new symbolic variables or data structures.
Similar to preconditioned symbolic execution, engines may also enable programmers to specify custom constraints for newly initialized data.

Some offline symbolic executors skip constraints with symbolic pointers during path generation but omit a part of the execution tree [@baldoni2018survey, sec. 3].
Engines that follow a hybrid strategy only concretize some symbolic addresses depending on the access type (read or write instructions), the number of possible results, and other heuristics.

## Constraint Solvers

Despite there is still no generic efficient algorithm known for the satisfiability problem, the performance of SMT solvers has significantly improved over the past decades.
Causes for this include advances regarding the underlying theories and algorithms for reducing constraints, deferring computation, and reusing solutions.
Solvers also benefit from increased hardware performance, parallelization, and acceleration using GPUs.

Another challenge is the generation of intuitive data during concretization.
For instance, some solvers favor numbers with small absolutes over random integers, or they stick with alphanumerical characters for strings and maximize repetition [@tillmann2014transferring, sec. 6.3].

## Trends in Symbolic Execution

Today, major challenges to symbolic execution include path explosion (\cref{path-management}) and solver performance (\cref{constraint-solvers}).
Several approaches employ the results of efficient static analysis to assist the symbolic execution engine with these challenges.
Another open problem is the analysis of concurrent programs with an exponential number of possible execution orders [@yang2019advances, sec. 7].

# Further Reading

Here, we recommend a shortlist of additional resources for deepening the field of symbolic execution.

Online resources.
:   [@luckow2017awesome] provides a collection of papers, educational materials, and implementations of symbolic execution techniques.
    [@papers] gives a more detailed overview of relevant papers.
    [@vartanov2017history] summarizes the history of symbolic execution engines and solvers in the form of graphical timelines.

Surveys.
:   [@cadar2013symbolic] gives an initial overview of symbolic execution approaches and implementations.
    [@baldoni2018survey] provides a comprehensive review of implementational techniques and considerations.
    [@yang2019advances] describes present challenges and trends.
    
    To handle the path explosion problem, search strategies are a much-noticed solution approach (\cref{path-selection}).
    [@liu2017survey] and [@sabbaghi2020systematic] survey this subfield of symbolic execution in detail.
