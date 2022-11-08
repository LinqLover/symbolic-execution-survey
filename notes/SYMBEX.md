# Symbolic Execution – Findings

## Approaches

### Classical symbolic execution

- maintain **symbolic state** (map of variables to **symbolic expressions**) and **path condition**/**path constraint** formula
- execute and refine path constraints/**fork** execution for them along **execution paths,** building an **execution tree**
- solve path constraint for each terminated path to provide concrete values

### Execution-generated Testing (EGT)

- concrete + symbolic execution (interleaving)
- guided by symbolic execution, operations without symbolic arguments are executed concretely
- for environment/blackbox calls: generate concrete values on demand using SMT solver

### Concolic testing

- *concolic* = <u>con</u>crete + symb<u>olic</u> execution (in parallel)
- concrete execution with real values guides symbolic execution for gathering constraints
  - start with random inputs
  - execute and record encountered constraints
  - negate last constraint that yields a new execution path
  - solve path constraint and try to generate new inputs
  - execute with new inputs
    - in case of non-deterministic program, assert that intended branch is reached (to deny **divergent execution**)
  - repeat recursively until no new constraints are found/computation limit is exceeded
- different search strategies (DFS, BFS, bounded DFS)
- depends on performant and powerful SMT solvers (NP-complete)
  - **SAT solver** (satisfiability): determines whether a equation system can be solved and provides a solution
  - **SMT solver** (satisfiability module („within“) theories): SAT solver for computer algebras with common data types
- comparison with classical symbex:
  - for environment/blackbox calls: can always continue with concrete values (but maybe omit some paths in the environment/blackbox)
  - for **unsolvable constraint sets:**
    - classical symbex needs to drop execution paths
      - (in theory, could also produce impossible execution paths)
    - concolic can always continue with concrete values (but omit some paths)
  - classic symbex needs its own executor and implement **forking,** concolic can **instrument** the program and reuse existing interpreter/compiler
    - environment calls from classic symbex execution forks might be out of order

## Applications

- modeling:
  - construction of CFGs (control flow graphs)
- program analysis:
  - check invariants to find bugs, uncaught exception, memory corruption, or security vulnerabilities
    - auto-fix bugs
  - find dead code
  - generate invariants
  - compare programs by execution trees
- test generation/reprocases for bugs
- dynamic recompilation

## Classification

- alternatives:
  - dynamic analysis (for single execution path with known inputs/without inputs)
  - static analysis (for static patterns)
  - manual code reading/testing
  - (non-whitebox) fuzzing
    - less systematic: either worse performance (for *all* possible inputs) or worse coverage than symbex
      - if there are less inputs than program paths, full-coverage fuzzing is faster (seldom)
    - symbiosis: whitebox-fuzzing (see SAGE)

## Tools

- DART: Directed Automated Random Testing (C, first implementation)
- CUTE: A Concolic Unit Testing Engine (C, jCUTE for Java)
  - concolic execution
  - DART + multithreading + dynamic data structures and pointers
- CREST (C, open platform)
- EXE (C)
  - bit-level accuracy (supports casts, including OS representations such as network packets, inodes, …)
- KLEE (C, open-source):
  - EXE for LLVM compiler
  - better memory performance
  - support for environment models (filesystem, …)
- Microsoft SAGE (x86 binaries)
  - concolic execution: simulate handling of corrupt files (1 symbol per byte)
- PEX
- S2E
- [PathCrawler](http://pathcrawler-online.com:8080/)

## Case Studies



## Challenges

### Path explosion

Large or possibly even infinite number of paths (symbolic break condition)

Solutions:
- limit execution time, number of paths, loop iterations, callstack size, or precision of symbolic representations
- heuristics for branch prioritization
  - to maximize statement/branch coverage:
    - favor paths closest to to any uncovered instruction from static CFG (obtained from static analysis)
    - favor statements that were run in symbex less often so far
  - randomization
  - random testing (test conditions for random inputs)
  - evolutionary search of test input space (genetic programming)
    - fitness based on results of static/dynamic analysis
    - TODO
  - mutation testing: prioritize random values based on mutation coverage
    - TODO
- merge paths by adding conditionals to symbolic expressions/constraints (increased SMT complexity)
- caches:
  - cache results (pre- and post-conditions) per function
  - discard execution at paths that were already reached with same constraints
- lazy test generation: top-down analysis by (initially) treating function calls as blackbox/unknown symbol?
  - TODO
- parallelization (split up search space)

### Memory modeling

- e.g., arrays, memory aliases, arithmetic overflow, pointers/OOPs
- increased model complexity -> increased computation complexity
- trade-off based on analysis goals (low-level vs high-level)
- controlling: choice of concretized values (for instance, by excluding `MAX_INT`, `nullptr`, etc.)

### Blackbox environment/nondeterministic behavior

Examples: syscalls, concurrency (multithreading, accelerators)

Solutions:

- no isolation (pollutes environment, unwanted side effects)
- emulation (complex models)
- fork environment (performance overhead)
- pass concrete values from concolic execution/EGT (may miss some execution paths through environment)

### Constraint Solving

Not efficiently solvable constraint sets (not implemented/blackbox expression/open-form expressions) when generating examples

Solutions:

- imprecision (approximated solutions, may miss some execution paths)
- eliminate irrelevant constraints: if only some constraints change (concolic execution), reuse all parts of the previous solution that the changed constraints do not depend on
- incremental solving: cache and reuse solutions for similar constraint sets (subsets: trivial, supersets: reduce number of unknown variables)

