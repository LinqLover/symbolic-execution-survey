# Symbolic Execution

## Introduction

- > From one point of view, implementation code has a “grammar” of the legal inputs it accepts and acts on, or rejects. [Symbolic execution] extracts this grammar (and the concrete sentences it accepts and rejects) from the implementation rather than from a hand-written specification.

- > [Symbolic execution] can be seen as a way to turn code “inside out” so that instead of consuming inputs [it] becomes a generator of them.

## Approaches for symbolic execution/symbex/SymEx

### Classical symbolic execution/pure symbolic execution

- maintain **symbolic state** (map of variables to **symbolic expressions**) and **path condition**/**path constraint** formula
- execute and refine path constraints/**fork** execution for them along **execution paths,** building an **execution tree**
- solve path constraint for each terminated path to provide concrete values
  - depends on performant and powerful SMT solvers (NP-complete)
    - **SAT solver** (satisfiability): determines whether a equation system can be solved and provides a solution
    - **SMT solver** (satisfiability module („within“) theories): SAT solver for computer algebras with common data types

### Dynamic symbolic execution (DSE)

mix concrete and symbolic execution

Motivation: improve performance, handle blackboxes

#### Execution-generated Testing (EGT)/online DSE

- concrete + symbolic execution (interleaving)
- guided by symbolic execution, operations without symbolic arguments are executed concretely
- for environment/blackbox calls: generate concrete values on demand using SMT solver

#### Concolic testing/offline DSE

- *concolic* = <u>con</u>crete + symb<u>olic</u> execution (simultaneously)

- concrete execution with real values guides symbolic execution for gathering constraints
  - start with random or existing inputs (e.g., existing test files, plausible input data)
  - execute and record encountered constraints
  - negate last constraint that yields a new execution path
  - solve path constraint and try to generate new inputs
  - schedule execution of program with new inputs
    - in case of non-deterministic program, assert that intended branch is reached (to deny **divergent execution**)
    - if supported by the executor, the execution can also be **forked**
  - repeat recursively until no new constraints are found/computation limit is exceeded

- different search strategies (DFS, BFS, bounded DFS (?), generational search)

- comparison with pure symbex:
  - for choosing next path, negate a single condition to alter concrete values
  
  - for environment/blackbox calls: can always continue with concrete values (but maybe omit some paths in the environment/blackbox)
  
  - for **unsolvable constraint sets:**
    - classical symbex needs to drop execution paths
      - (in theory, could also produce impossible execution paths)
      
    - concolic can always continue with concrete values (but omit some paths)
    
    - example:
    
      ```c
      if (hash(x) == s) { ... }
      ```
    
  - classic symbex needs its own executor and implement **forking,** concolic can **instrument** the program and reuse existing interpreter/compiler
    - environment calls from classic symbex execution forks might be out of order

### Static symbolic execution (SSE)

express entire program as a single symbolic expression (unless DSE, which has one constraint set per branch)

- less overhead for branches, more complex constraint sets
- strict SSE cannot deal with blackboxes and certain control flow patterns (why not?)

**veritesting:** mix DSE/concolic execution and SSE on program fragments for better performance

- often, complex queries can be solved more performant than branch overhead can be handled
- dynamic choosing between SSE and DSE
  - fall back to DSE for limitations of SSE (see above)
- implementation
  - start in DSE mode
  - on branch hit: recover CFG from path constraints, find *transition points* (before next SSE limitations, unknown/blackbox instructions, or function boundaries)
  - run SSE on reduced (acyclic) CFG: convert it into one large expression on the program state and solve that?
  - pass back control to DSE with the constraints and expressions developed for each transition point
- significant optimization! for a simple loop, makes difference between hours and seconds.
- similar to dynamic state merging, but that „still performs per-path execution, and only opportunistically merges“

## Applications

> potential of the tool

- modeling:
  - construction of CFGs (control flow graphs)
- program analysis:
  - find bugs/security vulnerabilities (uncaught exceptions, memory corruptions, violated assertions)
    - foundation for auto-fixing bugs
  - find dead code
  - infer invariants
  - compare programs by behavior
  - program exploration by input/output table
- test generation/reproduction of (non-deterministic) bugs
- dynamic recompilation (reverse engineering plus automated security patching/optimization)

## Classification

- technique for dynamic analysis
  - other than for classical dynamic analysis, no or no complete input data is required/symbex can find code paths itself
- alternatives:
  - static analysis (for static patterns)
  - analyze and test code manually
  - (non-whitebox) fuzzing
    - less systematic: either worse performance (for *all* possible inputs) or worse coverage than symbex
      - if there are less inputs than program paths, full-coverage fuzzing is faster (seldom)
    - symbiosis: whitebox-fuzzing (see SAGE)

## Tools

- DART: Directed Automated Random Testing (C, first implementation?)
  - concolic execution
- CUTE: A Concolic Unit Testing Engine (C, jCUTE for Java)
  - concolic execution
  - DART + multithreading + dynamic data structures and pointers
- CREST (C, open platform)
- EXE (C)
  - bit-level accuracy (supports casts, including OS representations such as network packets, inodes, …)
- KLEE (C, open-source):
  - EGT
  - EXE for LLVM compiler
  - better memory performance
  - support for environment models (filesystem, …)
- Microsoft SAGE (x86 binaries)
  - concolic execution: simulate handling of corrupt files (1 symbol per byte)
- Microsoft Intellitest
  - concolic (?) execution for C#/.NET Framework
  - previous names: PEX, Smart Unit Tests
  - based on Z3
  - Intellitest: tool in Microsoft Visual Studio (Enterprise) for test generation and exploration
    - parametrized unit test (PUT) framework
      - specify assumptions (preconditions) and assertions (postconditions)
      - specify parametrized mocks
      - provides mocks for many .NET components
    - symbolic types: automatic choice of possible concrete class for abstract types (abstract classes/interfaces)
    - settings for exploration bounds, choice of mocks, …
  - limitations: nondeterminism, concurrency, native code, constraint solving, …
  - practical consequences:
    - pro: even without actual unit tests with assertions, exploration helps reveal forgotten code paths/exceptions
    - can use assertions in the code base / still, without assertions, unexpected behavior may be missed
    - con: symbex of even simple framework calls reveals large complexity – e.g., a `Console.WriteLine();` might throw several exceptions
      - false positives due to blackbox implementation of `Console`
      - possibly distracts from actual business logic, need to ignore many exceptions, quality vs quantity
- S2E: research platform for symbex of binaries
- [PathCrawler](http://pathcrawler-online.com:8080/)
- Java Symbolic PathFinder
- MergePoint (x86 binaries)
  - veritesting
  - impact: checked all 33k debian binaries in 18 CPU-months revealed 11k bugs (Amazon EC2: \$0.28/bug)

==TODO: Make this a table with columns for target platform, implementation approach, and remarkable notes/impacts, if any==

## Case Studies



## Challenges

### Path explosion

aka **state explosion** („state“ = program path)

Large or possibly even infinite number of paths (loops/recursion with symbolic break condition)

Solutions:
- reduce number of paths through path merging/veritesting
  - (increased SMT complexity, but usually worth)
- trade-in precision/coverage for sake of runtime through (configurable) limits:
  - execution time, number of paths, loop iterations, callstack size
  - precision of symbolic representations
  - selective symbolic execution (SSE): which parts of program to analyze symbolically
- search strategies/branch prioritization
  - maximize statement/branch coverage:
    - favor paths closest to to any uncovered instruction from static CFG (obtained from static analysis)
    - favor statements that were run in symbex less often so far
  - randomization (?)
  - random testing (test conditions for random inputs?)
  - evolutionary search of test input space (genetic programming)
    - fitness based on results of static/dynamic analysis
    - TODO
  - mutation testing: prioritize random values based on mutation coverage
    - TODO
  - BFS/DFS
    - TODO
- caches:
  - cache results (pre- and post-conditions) per function
  - discard execution at paths that were already reached with same constraints
- lazy test generation: top-down analysis by (initially) treating function calls as blackbox/unknown symbol?
  - TODO
- parallelization (split up search space, run on multiple CPUs/nodes)

### Memory modeling

- e.g., arrays, memory aliases, arithmetic overflow, pointers/OOPs

- the problem with symbolic memory access (e.g., array element at symbolic index): only accurately solvable by concretizing symbolic address

  - for concolic execution, concrete value may be used, but some branches may be omitted

  - example:

    ```c
    void divergent(int x, int y)
    {
    	int s[4];
    	s[0] = x;
    	s[1] = 0;
    	s[2] = 1;
    	s[3] = 2;
    	if (s[x] = s[y] + 2) {
    		abort(); //error
    	}
    }
    ```

- increased model complexity -> increased computation complexity

- trade-off based on analysis goals (low-level vs high-level)

- controlling: choice of concretized values (for instance, by excluding `MAX_INT`, `nullptr`, etc.)

- TODO – how do solutions and sophisticated memory models look like?

### Blackbox environment/nondeterministic behavior/path divergence

Examples: syscalls, concurrency (multithreading, accelerators), frameworks (inversion of control)

Solutions:

- pass concrete values from DSE to blackbox without isolation (pollutes environment, unwanted side effects, limited code coverage)
- emulation (complex models): mocks/environment drivers for providing state in symbex and reproducing it in generated tests
- fork environment (huge performance overhead)
- heuristic approaches: combine symbex with sub-callgraphs/fuzzing (see „Testing Android Apps“)

### Limitations of Constraint Solver

Not efficiently solvable constraint sets (missing theory knowledge / efficient algorithm not known/existing/implemented / blackbox expression)

- examples: hash, prime factorization, …

Solutions:

- imprecision (approximated solutions, may miss some execution paths)
- optimizations for brute-forcing
  - eliminate irrelevant constraints: if only some constraints change (concolic execution), reuse all parts of the previous solution that the changed constraints do not depend on
  - incremental solving: cache and reuse solutions for similar constraint sets (subsets: trivial, supersets: reduce number of unknown variables)

Value concretization: prefer simple and human-readable values (e.g., `1` over `4328902461` or `abc` over `°╚Ã`)
