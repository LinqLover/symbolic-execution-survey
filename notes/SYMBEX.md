# Symbolic Execution

## Introduction

aka **symbex,** **SymEx**

- > From one point of view, implementation code has a “grammar” of the legal inputs it accepts and acts on, or rejects. [Symbolic execution] extracts this grammar (and the concrete sentences it accepts and rejects) from the implementation rather than from a hand-written specification.

- > [Symbolic execution] can be seen as a way to turn code “inside out” so that instead of consuming inputs [it] becomes a generator of them.

Formal specification:

- Inputs:
  - executable program (source code/bytecode/binary)
  - optional configuration (time constraints, priority schemes, assumptions – see challenges below)
  - optional environment models (drivers/mocks/stubs – see challenges below)
- Outputs:
  - option 1: set of alternative inputs for the program
    - may contain function parameters, global variables, environment behavior
    - should maximize the code coverage for the program and minimize the number of inputs
    - optimal: should minimize complexity of inputs
  - option 2: set of input constraints instead of concrete values
  - option 3: results from executing a given function on each encountered program state

## Classification

- technique for dynamic analysis
  - other than for traditional dynamic analysis, no or no complete input data is required/symbex can find code paths itself
- alternatives:
  - static analysis (for static patterns)
    - no context available or harder to reconstruct
  - analyze and test code manually
    - not automated
  - (non-whitebox) fuzzing
    - less systematic: either significantly worse performance (for *all* possible inputs) or worse coverage than symbex
      - if there are less inputs than program paths, full-coverage fuzzing is faster (applies very seldom)
    - symbiosis: whitebox-fuzzing (see SAGE)

## Applications

> potential of the tool

- program analysis:
  - find bugs/security vulnerabilities (uncaught exceptions, memory corruptions, violated contracts/assertions)
    - foundation for auto-fixing bugs
    - exploit generation (e.g., AEG)
    - smart contract verification (e.g., Mythril)
  - find dead code
  - formal verification/check invariants
  - infer invariants
  - compare programs by behavior
    - contractual SemVer (e.g., CrossHair)
- reverse engineering
  - generate input/output table (e.g., Pex/IntelliTest)
  - construction of CFGs (control flow graphs)
  - disassemblers (e.g., medusa)
  - symbolic execution debugging (e.g., SED, Ponce)
- testing
  - crash test generation/reproduction of (non-deterministic) bugs
    - no assertions
  - testing frameworks (e.g., Pex/IntelliTest, DeepState)
  - (minimum set of inputs is not always ideal: especially for well-maintainable fixtures, a larger set of inputs could be more intuitive, self-documenting, and coverage-robust against changes in the implementation)
- dynamic recompilation (e.g., BinRec: reverse engineering plus automated security patching/optimization)

## Implementations

(selections)

### Execution Engines

<table>
    <thead>
        <tr>
            <td>Name</td>
            <td>Release Date</td>
            <td>Languages/platforms</td>
            <td>Implementation approach</td>
            <td>Highlights</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><strong>DART: Directed Automated RandomTesting</strong></td>
            <td>2005</td>
            <td>C</td>
            <td>concolic execution</td>
            <td>
                <ul>
                    <li>first implementation ever?</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>CUTE: A Concolic Unit Testing Engine</strong></td>
            <td>2005</td>
            <td>C</td>
            <td>concolic execution</td>
            <td>
                <ul>
                    <li>DART + multithreading + dynamic data structures and pointers</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>EXE</strong></td>
            <td>2006</td>
            <td>C</td>
            <td>concolic execution</td>
            <td>
                <ul>
                    <li>bit-level accurate memory model (supports casts, including OS representations such as network packets, inodes, …)</li>
                    <li>successful for discovering bugs/vulnerabilities in different areas such as libraries, file systems, drivers, …</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>jCUTE</strong></td>
            <td>2006</td>
            <td>Java</td>
            <td>concolic execution</td>
            <td></td>
        </tr>
        <tr>
            <td><strong>CREST</strong></td>
            <td>2008</td>
            <td>C</td>
            <td>concolic execution</td>
            <td>
                <ul>
                    <li>open platform?</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>KLEE</strong></td>
            <td>2008</td>
            <td>C</td>
            <td>EGT</td>
            <td>
                <ul>
                    <li>EXE for LLVM compiler (?)</li>
                    <li>better memory performance</li>
                    <li>support for environment models (filesystem, ...)</li>
                    <li>impact: 90% code coverage for coreutils</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>Microsoft SAGE</strong></td>
            <td>2008</td>
            <td>x86 binaries (and maybe others)</td>
            <td>concolic execution</td>
            <td>
                <ul>
                    <li>based on Z3 (SMT solver)</li>
                    <li>simulate handling of corrupt files (degenerate symbolic bytes from test files)</li>
                    <li>impact: responsible for finding 1/3 of bugs in Windows 7, standard component of Microsoft's internal testing pipelines, run daily 24/7 on more than 200 machines</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>Microsoft PEX</strong></td>
            <td>2008</td>
            <td>.NET Framework (C#, F#, VB.NET)/CIL bytecode?</td>
            <td>concolic execution (?)</td>
            <td>
                <ul>
                    <li>based on Z3 (SMT solver)</li>
                    <li>common limitations: nondeterminism, concurrency, native code, constraint solving, …</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>Java Symbolic PathFinder</strong></td>
            <td>2010</td>
            <td>Java</td>
            <td>pure symbolic execution</td>
            <td>
                <ul>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>S2E</strong></td>
            <td>2011</td>
            <td>x86 binaries</td>
            <td>online symbolic execution</td>
            <td></td>
        </tr>
        <tr>
            <td><strong>Mayhem</strong></td>
            <td>2012</td>
            <td>x86 binaries</td>
            <td>hybrid symbolic execution (alternating online + concolic)</td>
            <td></td>
        </tr>
        <tr>
            <td><strong>MergePoint</strong></td>
            <td>2014</td>
            <td>x86 binaries</td>
            <td>veritesting</td>
            <td>
                <ul>
                    <li>impact: checked all 33k debian binaries in 18 CPU-months revealed 11k bugs (Amazon EC2: $0.28/bug)</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong>angr</strong></td>
            <td>2016</td>
            <td>binaries</td>
            <td>concolic execution</td>
            <td></td>
        </tr>
    </tbody>
</table>

More engines and solvers: <https://github.com/enzet/symbolic-execution>

### Tools

<table>
    <thead>
        <tr>
            <td>Name</td>
            <td>Release Date</td>
            <td>Languages/platforms</td>
            <td>Implementation</td>
            <td>Highlights</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><strong><a href="https://github.com/wisk/medusa">medusa</a></strong></td>
            <td>2011</td>
            <td>Assembler</td>
            <td></td>
            <td>interactive disassembler</td>
        </tr>
        <tr>
            <td><strong><a href="http://pathcrawler-online.com:8080/">PathCrawler</a></strong></td>
            <td>2013</td>
            <td>C</td>
            <td></td>
            <td>education/demo of operating principle, visualization of code coverage</td>
        </tr>
        <tr>
            <td><strong><a href="https://docs.idaponce.com/examples/symbolic-engine">Ponce</a></strong></td>
            <td>2013</td>
            <td>Assembler</td>
            <td></td>
            <td>constraint-injection-based debugging for exploration of binaries</td>
        </tr>
        <tr>
            <td><strong>Microsoft IntelliTest</strong><br>(previously aka <strong>Smart Unit Tests</strong>)</td>
            <td>2015</td>
            <td>.NET Framework (C#, F#, VB.NET)</td>
            <td>based on Microsoft PEX</td>
            <td>
                <ul>
                    <li><strong>Parametrized Unit Testing (PUT)</strong> framework: specify <em>assumptions</em> (preconditions) and <em>assertions</em> (postconditions), create parametrized mocks, and use provided mocks for many .NET components</li>
                    <li>
                        <strong>test exploration:</strong>
                        <ul>
                            <li>pro: even without actual unit tests with assertions, exploration helps reveal forgotten code paths/exceptions</li>
                            <li>can use assertions in the code base / still, without assertions, unexpected behavior may be missed</li>
                            <li>
                                con: symbex of even simple framework calls reveals large complexity – e.g., a <code>Console.WriteLine();</code> might throw several exceptions
                                <ul>
                                    <li>false positives due to blackbox implementation of <code>Console</code></li>
                                    <li>possibly distracts from actual business logic, need to ignore many exceptions, quality vs quantity</li>
                                </ul>
                            </li>
                            <li>con: even for exploration, need to specify factories</li>
                        </ul>
                    </li>
                    <li>symbolic types: automatic choice of possible concrete class for abstract types (abstract classes/interfaces)</li>
                    <li>configuration options for exploration bounds, choice of mocks, …</li>
                    <li>impact: part of Microsoft Visual Studio (Enterprise) since 2015</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong><a href="https://pypi.org/project/crosshair-tool/">CrossHair</a></strong></td>
            <td>2017</td>
            <td>Python</td>
            <td></td>
            <td>
                <ul>
                    <li>interactive contract checking</li>
                    <li>test generation</li>
                    <li>behavioral diffing/contractual SemVer</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td><strong><a href="https://github.com/ConsenSys/mythril">mythril</a></strong></td>
            <td>2017</td>
            <td>Ethereum smart contracts (Solidity, Yul, Vyper, ...) for EVM</td>
            <td></td>
            <td>security analysis tool for EVM bytecode (smart contract checking)</td>
        </tr>
        <tr>
            <td><strong><a href="https://github.com/trailofbits/deepstate">DeepState</a></strong></td>
            <td>2018</td>
            <td>C/C++</td>
            <td></td>
            <td>parametrized unit testing framework</td>
        </tr>
        <tr>
            <td><strong><a href="https://www.key-project.org/applications/debugging/">Symbolic Execution Debugger</a></strong></td>
            <td>2019</td>
            <td>Java</td>
            <td></td>
            <td>interactive exploration of symbolic execution tree</td>
        </tr>
        <tr>
            <td><strong><a href="https://github.com/microsoft/onefuzz">OneFuzz</a></strong></td>
            <td>2020</td>
            <td>binaries</td>
            <td>based on Microsoft SAGE</td>
            <td>automatic fuzzing tool with means for reproduction and debugging</td>
        </tr>
    </tbody>
</table>

## Impact

### Security Testing

- SAGE at Microsoft: responsible for finding 1/3 of bugs in Windows 7, standard component of Microsoft's internal testing pipelines, run daily 24/7 on more than 200 machines
  - each security bulletin costs multiple millions USD
- coreutils (89 binaries, 72 kLOC, originally 67.6% LCOV):
  - KLEE (2008): 84.5% LCOV, 56 bugs/89 h
  - zesti (2012): 8 bugs/22 h
  - efficient state merging (2012): 89 h
  - Mayhem (2012): 97.6% LCOV/25 h (for subset of 25 binaries)
- Debian (33k binaries):
  - MergePoint (2014): 11k bugs/18 CPU-months (Amazon EC2: \$0.28/bug)

### Tooling

- testing and exploration:
  - IntelliTest in Visual Studio (used by millions of developers)
  - CrossHair (>800 stars on GitHub)
  - DeepState (>700 stars on GitHub)
- program analysis platforms
  - angr (>6.3k stars on GitHub): binary analyis platform with Python API; symbolic execution; disassembly & decompilation
  - Manticore (>3.3k stars on GitHub): symbolic analysis of smart contracts and binaries with Python API
  - Miasm (>3k stars on GitHub): binary analysis, modification, and generation
  - mythril (>2.9k stars on GitHub): security analysis tool for EVM bytecode (smart contract checking)
  - Triton (>2.7k stars on GitHub): dynamic binary analysis library
  - OneFuzz (>2.6k stars on GitHub): automatic fuzzing tool with means for reproduction and debugging
  - Binary Analysis Platform (BAP) (>1.8k stars on GitHub)
  - SymCC (>600 stars on GitHub): symbolic execution compiler wrapper for C/C++; test generation
  - Maat (>500 stars on GitHub): binary analysis framework; environment simulation
  - Alive2 (>400 stars on GitHub): verification of LLVM optimizations
  - Jalangi2 (>300 stars on GitHub): dynamic analysis framework for JavaScript; suspicious bug pattern detection (propagation of `NaN`s an `undefined`s, …); memory profiler; JIT-unfriendly code snippet detection
  - FuzzBALL (>200 stars on GitHub): binary symbolic execution engine
  - S2E (>200 stars on GitHub): binary analysis platform
- disassembly: medusa (>1k stars on GitHub), Ponce (>1.3k stars on GitHub)

## Implementation approaches

### Traditional symbolic execution/pure symbolic execution

- maintain **symbolic state** (map of variables to **symbolic expressions**) and **path condition**/**path constraint** formula
- execute and refine path constraints/**fork** execution for them along **execution paths,** building an **execution tree**/**control-flow graph (CFG)**
- solve path constraint for each terminated path to provide concrete values
  - depends on performant and powerful SMT solvers (NP-complete)
    - **SAT solver** (satisfiability): determines whether a equation system can be solved and provides a solution
    - **SMT solver** (satisfiability modulo („within“) theories): SAT solver for computer algebras with common data types

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

    - traditional symbex needs to drop execution paths

      - (in theory, could also produce impossible execution paths)

    - concolic can always continue with concrete values (but omit some paths)

    - example:

      ```c
      if (hash(x) == s) { ... }
      ```

  - traditional symbex needs its own executor and implement **forking,** concolic can **instrument** the program and reuse existing interpreter/compiler

    - environment calls from traditional symbex execution forks might be out of order

### Static symbolic execution (SSE)

express entire program as a single symbolic expression (unless DSE, which has one constraint set per branch)

- less overhead for branches, more complex constraint sets
- strict SSE cannot deal with blackboxes and certain control flow patterns (why not?)
- TODO: how exactly does it work? just an AST transformation?

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

### Backward symbolic execution

TODO

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
  - **selective symbolic execution:** which parts of program to analyze symbolically
  - **directed symbolic execution:** find parts of program close to unit of interest
- search strategies/branch prioritization
  - **depth-first search (DFS):** intuitive/simple implementation, not loop-safe
  - **breadth-first search (BFS):** too many context switches, memory overhead
  - **random search:** not always loop-safe
  - **generational search** (SAGE): negate each constraint from current branch separately, smaller memory overhead
  - heuristics:
    - order branches by code coverage increase
    - CFG-based branch order (CarFast)
      - favor paths closest to to any uncovered instruction from static CFG (obtained from static analysis)
    - evolutionary search of test input space (genetic programming)
      - fitness based on results of static/dynamic analysis
      - TODO
  - random testing (test conditions for random inputs?)
  - mutation testing: prioritize random values based on mutation coverage
    - TODO
- caches:
  - cache results (pre- and post-conditions) per function
  - discard execution at paths that were already reached with same constraints
- lazy test generation: top-down analysis by (initially) treating function calls as blackbox/unknown symbol?
  - TODO
- parallelization (split up search space, run on multiple CPUs/nodes)

### Memory modeling

- symbolic reasoning of, e.g., arrays, memory aliases, arithmetic overflow, pointers/OOPs/function pointers

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

- **generalized symbolic execution**/**lazy initialization** of pointers

- TODO – how do solutions and sophisticated memory models look like?

### Blackbox environment/nondeterministic behavior/path divergence

Examples: syscalls, concurrency (multithreading, accelerators), frameworks (inversion of control)

Solutions:

- pass concrete values from DSE to blackbox without isolation (pollutes environment, unwanted side effects, limited code coverage)
- emulation (complex models): mocks/environment drivers for providing state in symbex and reproducing it in generated tests
- fork environment (huge performance overhead)
- heuristic approaches: combine symbex with sub-callgraphs/fuzzing (TODO) (see „Testing Android Apps“)

### Limitations of Constraint Solver

Not efficiently solvable constraint sets (missing theory knowledge / efficient algorithm not known/existing/implemented / blackbox expression)

- examples: hash, prime factorization, …

Solutions:

- imprecision (approximated solutions, may miss some execution paths)
- optimizations for brute-forcing
  - eliminate irrelevant constraints: if only some constraints change (concolic execution), reuse all parts of the previous solution that the changed constraints do not depend on
  - incremental solving: cache and reuse solutions for similar constraint sets (subsets: trivial, supersets: reduce number of unknown variables)

Value concretization: prefer simple and human-readable values (e.g., `1` over `4328902461` or `abc` over `°╚Ã`)
