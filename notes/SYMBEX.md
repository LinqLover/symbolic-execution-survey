# Symbolic Execution – Findings

## Approaches

### Classical symbolic execution

- maintain **symbolic state** (map of variables to **symbolic expressions**) and **path condition**/**path constraint** formula
- execute and refine path constraints/**fork** execution for them along **execution paths,** building an **execution tree**
- solve path constraint for each terminated path to provide concrete values

### Concolic testing

- *concolic* = <u>con</u>crete + symb<u>olic</u> execution:
  - start with random inputs
  - execute and record encountered constraints
  - negate last constraint that yields a new execution path
  - solve path constraint and try to generate new inputs
  - execute with new inputs
    - in case of non-deterministic program, assert that intended branch is reached (to deny **divergent execution**)
  - repeat recursively until no new constraints are found
- different search strategies (DFS, BFS, bounded DFS)
- depends on performant and powerful SMT solvers
  - **SAT solver** (satisfiability): determines whether a equation system can be solved and provides a solution
  - **SMT solver** (satisfiability module („within“) theories): SAT solver for computer algebras with common data types
- comparison with classical symbex:
  - for environment/blackbox calls:
    - classical symbex needs to drop execution paths
    - concolic can always continue with concrete values (but maybe omit some paths in the environment/blackbox)
  - for **unsolvable constraints:**
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
- provide concrete values to generate regression tests/reprocases for bugs
- dynamic recompilation

## Classification

- alternatives:
  - dynamic analysis (for known inputs/without inputs)
  - static analysis (for static patterns)
  - manual code reading/testing
  - (non-whitebox) fuzzing
    - less systematic: either worse performance (for *all* possible inputs) or worse coverage than symbex
      - if there are less inputs than program paths, full-coverage fuzzing is faster (seldom)
    - symbiosis: whitebox-fuzzing (see SAGE)

## Tools

- concolic testing:
  - Microsoft SAGE
    - concolic execution: simulate handling of corrupt files (1 symbol per byte)
  - KLEE
  - S2E
  - [PathCrawler](http://pathcrawler-online.com:8080/)

## Limitations

- **path explosion**
  - possibly even infinite number of paths (symbolic break condition)
  - solution:
    - limit execution time, number of paths, loop iterations, callstack size, or precision of symbolic representations
    - heuristics for branch scheduling (random, coverage-based, priorities for code regions)
    - choice of the right heuristics is still an open challenge
- heap modeling (e.g., arrays, memory aliases, OOPs) (increased model complexity)
- interactions with blackbox **environment** /**nondeterministic behavior**
  - solutions:
    - no isolation (pollutes environment, unwanted side effects)
    - emulation (complex models)
    - fork environment (performance overhead)
    - concolic execution (see above)
- not efficiently solvable constraints (not implemented/blackbox expression/open-form expressions) when generating examples

