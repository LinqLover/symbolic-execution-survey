---
title: "Symbolic Execution and Applications"
author: [Christoph Thiede]
titlepage: true
abstract: |
  TODO: Abstract
mainfont: "Georgia"
sansfont: "Georgia"
filters:
- pandoc-fignos
header-includes: |
  \usepackage{float}
  \newfloat{lstfloat}{htbp}{lop}
  \floatname{lstfloat}{Listing}
  \def\lstfloatautorefname{Listing}
# lstfloatautorefname might be needed for hyperref/auroref
...

# Introduction

*Program analysis* covers a wide range of techniques and programming tools for generating insights about the structure and the behavior of a software system.
We can distinguish between *static* and *dynamic* analysis tools:
static analysis tools examine a program by its source code to derive particular information (e.g., dependency analysis, type flow analysis) or detect occurrences of relevant patterns (e.g., spell checkers, linters).
Dynamic analysis tools, on the other hand, examine a program by observing the behavior of the running application (e.g., tests, benchmarks, or assertions).

While static analysis typically offers a higher performance, it often provides a lower precision or selectivity than dynamic analysis.
For instance, static type flow analyzers are unable to reason about metaprogramming constructs such as Python's `getattr` function or JavaScript's `Function.prototype.bind()` method, and static performance prediction tools cannot respect bottlenecks of an individual hardware setup.
However, while dynamic analyis tools typically offer a higher quality of results, they require *context* for running an application and reaching all its different states.
Programmers can provide context by composing a set of possible user interactions or creating a comprehensive test suite, but this is a costly process and often, these artifacts do not exist or only cover a small portion of the code base.

*Symbolic execution* addresses this issue by automatically uncovering most execution paths of a program and performing analyses on them while requiring no or fewer specifications from programmers.
The fundamental operating principle of symbolic execution is to execute a program with *symbolic variables* instead of concrete values for its inputs and to fork the execution of the program every time a conditional expression (e.g., in an `if` statement) can have different results depending on the assignment of the symbolic variables.

In the last four decades, programmers and researchers have applied symbolic execution for different purposes such as vulnerability analysis of programs, program verification, unit testing, or various program understanding tasks such as model generation or reverse engineering.

In this report, we give a summary of several implementations and applications of symbolic execution.
In \cref{approach}, we describe the fundamental operating principle of symbolic execution.
In \cref{implementation-strategies}, we provide an overview of challenges for symbolic execution and existing solution approaches.
We present different use cases and tools for symbolic execution in \cref{applications} and examine their impact in research and industry in \cref{impact}.
In \cref{discussion}, we discuss some limitations and usage considerations for different symbolic execution tools.
We classify some alternative approaches and literature in \cref{related-work} and finally give a conclusion in \cref{conclusion}.

# Approach

In this section, we describe the general concept of symbolic execution and introduce neccessary definitions.

*Symbolic execution* (also abbreviated to *symbex* or *SymEx*) attempts to systematically uncover all execution paths of a program.
It takes a program in an executable form (e.g., x86 binary, LLVM bitcode, or JVM bytecode) and produces a list of inputs that trigger different code paths in the program.
Cadar describes it as a technique "to turn code 'inside out' so that instead of consuming inputs \[it\] becomes a generator of them" [@cadar2005execution].

To this end, the *symbolic execution engine* or the *symbolic executor* runs the program with its normal execution semantics but assigns a special *symbolic variable* to each input:

- As the program performs arithmetic or logic operations on symbolic variables, they are composed to *symbolic expressions* that can be propagated through the *symbolic memory* or *symbolic store* of the program execution.
- As the program hits a branch condition such as an `if` statement that depends on a symbolic expression, the executor decides the possible results of the depended expression (e.g., `true` or `false`).
  If the expression does not have a single solution, the entire execution is *forked* into multiple *execution paths* that assume a different result of the expression, and each fork is assigned a new *path constraint* for the assignment of its symbolic variable in the form of a Boolean first-order expression.

Together, the symbolic memory and the constraint set of an execution path constitute its *symbolic state*.
The entirety of execution paths is named the *symbolic execution tree* of the program where each node represents a symbolic state and each edge represents a new path constraint.
For each leaf, i.e., each completed execution path, the symbolic execution engine can generate a concrete set of input values from the contraint set that programmers can use to reproduce the same execution path in a regular non-symbolic context.

A crucial component of each symbolic execution engine is an *SMT solver* that decides whether a symbolic branch condition can be fulfilled and generates a concrete solution for a constraint set.
SMT solvers are a special kind of *SAT solvers* that test the *satisfiability* of a constraint set *modulo* ("within") a set of *theories*.
Theories are axiomatic systems for domain-specific algebras that enable the solver to reason not only about Boolean logic but also about predicates involving various datatypes.
Popular theories describe arithmetics, bitwise operations/integers with overflow semantics, or strings.

\begin{lstfloat}
  \begin{lstlisting}[language=c, label=lst:foo-code, caption={A simple method in C that can be executed symbolically.}]
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

![The symbolic execution tree for analyzing the `foo()` method from \cref{lst:foo-code}. The symbolic variables $\alpha, \beta, \gamma$ represent the assigned values for the input variables `a`, `b`, and `c`.](./figures/symbex-tree.png){#fig:foo-tree}

For example, a symbolic execution of the `foo()` function in \cref{lst:foo-code} would produce the symbolic execution tree displayed in +@fig:foo-tree.

# Implementation Strategies

# Applications

# Impact

# Discussion

# Related Work

# Conclusion

<!--
# References

<div id="refs"></div>
-->
