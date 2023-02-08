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
      - if there are fewer inputs than program paths, full-coverage fuzzing is faster (applies very seldom)
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
            <td>concolic execution, DFS, address concretization</td>
            <td></td>
        </tr>
        <tr>
            <td><strong>CUTE: A Concolic Unit Testing Engine</strong></td>
            <td>2005</td>
            <td>C</td>
            <td>concolic execution, address concretization</td>
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
            <td></td>
        </tr>
        <tr>
            <td><strong>KLEE</strong></td>
            <td>2008</td>
            <td>LLVM (C, ...)</td>
            <td>EGT, online symbolic execution</td>
            <td>
                <ul>
                    <li>cover environment by using x86-to-LLVM lifter</li>
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
            <td>concolic execution, generational search</td>
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
            <td>concolic execution</td>
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
            <td><strong>S²E</strong></td>
            <td>2011</td>
            <td>x86 binaries</td>
            <td>online symbolic execution, selective symbolic execution, virtualized environment</td>
            <td></td>
        </tr>
        <tr>
            <td><strong>Mayhem</strong></td>
            <td>2012</td>
            <td>x86 binaries</td>
            <td>hybrid symbolic execution, partial memory model</td>
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

## Implementation Approaches

### Basics

**Symbolic execution:**

- execute a program with **symbolic variables** instead of concrete inputs
- all accesses to symbolic variables create **symbolic expressions**
- during execution, maintain a **symbolic state:**
  - **symbolic store**/**symbolic memory** (map of variables to symbolic expressions)
  - **path condition**/**path constraint** formula (set of Boolean expressions over symbolic variables)
- use **branch conditions** that depend on a symbolic expression to uncover **execution tree:**
  - paths: all discovered **execution paths** through the program (represent equivalences class of inputs)
  - states: branch conditions at symbolic states
  - edges: new constraints for different results of parent symbolic branch condition
- to provide input values for different paths, solve path constraints for each terminated path

**SMT solver:**

- find possible results and solve path constraints (NP-complete)

- **SAT solver** (satisfiability): decides whether a equation system can be solved and provides concrete solutions if possible

- **SMT solver** (satisfiability modulo („within“) theories): SAT solver for computer algebras with common data types
  - domain-specific theories (arithmetic, integer overflows, bitvectors, strings, …)
  - popular implementations: Z3 (de-facto industry standard), STP
  
- <details>
  <summary>Implementation (beta)</summary>

  - Applies transformations (**tactics**) to simplify/normalize formula
    - examples: constant folding ($x + 0 \to x$), normalize bounds ($k \leq x \to 0 \leq x - k$), bit blasting (split up variable into bits)
  - Evaluate heuristics (**predicates** on formula expression) to select tactics
    - handcrafted or learned
  - Algorithms:

    - **DPLL algorithm** for solving boolean expressions in CNF
      - backtracking – divide and conquer over set of variables
      - identify and solve **unit clauses** that only contain single unknown literal (solvable in in constant time), backtrack if mismatch
    </details>

### Offline vs Online Execution

#### Offline/Traditional/Pure Symbolic Execution

- when encountering a symbolic branch condition, **fork** execution for all possible results into different execution paths
- efficient cloning of symbolic state through **copy-on-write** (shared symbolic stores for forks)
- different search strategies for prioritizing execution paths (see [Challenges](#path-explosion))

#### Online Symbolic Execution

- instead of forking, execute one symbolic state at a time
- when a path is completed, start again from the beginning of the program
- pros: smaller memory footprint, no state isolation between forks required
- con: repeated work for each path

#### Hybrid Symbolic Execution

online until memory bounds hit, then checkpoints for new branches, resume from them later online

### Dynamic Symbolic Execution (DSE)

mix concrete and symbolic execution

Motivation: improve performance, handle blackboxes

#### Execution-generated Testing (EGT) (online)

- concrete + symbolic execution (interleaving)
- guided by symbolic execution, operations without symbolic arguments are executed concretely
- for environment/blackbox calls: generate concrete values on demand using SMT solver

#### Concolic Testing (offline)

- *concolic* = <u>con</u>crete + symb<u>olic</u> execution (simultaneously)

- concrete execution with real values guides symbolic execution for gathering constraints

  - start with random or existing input seed(s) (e.g., existing test files, plausible input data)
    - aka **„whitebox fuzzing“**
  - execute and record encountered constraints
  - negate last constraint that yields a new execution path
  - solve path constraint and try to generate new inputs
  - schedule execution of program with new inputs
    - in case of non-deterministic program, assert that intended branch is reached (to deny **divergent execution**)
    - if supported by the executor, the execution can also be **forked**
  - repeat recursively until no new constraints are found/computation limit is exceeded

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


### Static Symbolic Execution (SSE)

express entire program as a single symbolic expression (unlike DSE, which has one constraint set per branch)

- AST transformation using **function summaries** and **loop summaries**
- less overhead for branches, more complex constraint sets
- strict SSE cannot deal with blackboxes and complex control flow patterns

**Veritesting:** mix DSE/concolic execution and SSE on program fragments for better performance

- often, complex queries can be solved more performant than branch overhead can be handled
- dynamic choosing between SSE and DSE
  - fall back to DSE for limitations of SSE (see above)
- implementation
  - start in DSE mode
  - on branch hit: recover CFG from path constraints, find *transition points* (before next SSE limitations, unknown/blackbox instructions, or function boundaries)
  - run SSE on reduced (acyclic) CFG: convert it into one large expression on the program state and solve that
  - pass back control to DSE with the constraints and expressions developed for each transition point
- significant optimization! for a simple loop, makes difference between hours and seconds.
- similar to dynamic state merging, but that „still performs per-path execution, and only opportunistically merges“

### Backward Symbolic Execution (BSE)

used for reverse debugging

- **call-chain backward symbolic execution:** explore possible callers of function
  - uses static CFG between functions
  - path explosion!

## Challenges

### Path Explosion

aka **state (space) explosion** („state“ = program path)

Large or possibly infinite number of paths (loops/recursion with symbolic break condition)

Solutions:
- reduce number of paths through **path summarization/pruning/merging**
  - **function summaries:** create conditional symbolic expressions
    - uses **compositional analysis:** analyze units separately and store pre- and postconditions
  - **loop summaries**
    - compact templates for loop bodies
    - prove loop invariants
    - challenges: multi-path loops, irregular side effects, …
  - cache summaries for later
  - **path pruning/subsumption** of similar paths
    - ignore differences in return values/side effects that are never read
    - remember error locations with constraints and do not retry in another branch
  - **postconditioned symbolic execution:** incremental construction of weakest postconditions for program locations by backtracking complete paths (?)
    - efficiency depends on path selection strategy (requires complete paths)
    - uses **interpolation** (?)
  - abstractions that summarize memory and constraints
  - **path merging:** conditional constraints, conditional expressions in symbolic memory
    - heuristics for choosing similar paths worth merging
      - also may predict solver costs
      - consider static CFG
    - veritesting
  - (increased SMT complexity, but usually worth)
- trade-in precision/coverage for sake of runtime through (configurable) limits:
  - execution time, number of paths, loop iterations, callstack size
  - precision of symbolic representations
  - **selective symbolic execution:** user-specified parts of program to analyze symbolically
    - **preconditioned symbolic execution:** user-specified preconditions (i.e., buffer sizes, known bytes, grammars for inputs)
    - reduces completeness
  - **directed/shortest-distance symbolic execution:** find parts of program close to unit of interest
  - **under-constrained symbolic execution** of individual functions: false positives for never-met constraints
  - **lazy test generation:** top-down analysis by (initially) treating function calls as blackbox/unknown symbol
  - **shadow symbolic execution:** in CI context, exploit differences of older program version and symbex results for that
- search strategies/branch prioritization/path selection
  - **depth-first search (DFS):** intuitive/simple implementation, not loop-safe
  - **breadth-first search (BFS):** too many context switches, memory overhead
  - **random search:** not always loop-safe
  - **generational search** (SAGE): negate each constraint from current branch separately, smaller memory overhead
  - heuristics:
    - weight/order branches by number of visits or code coverage/mutation coverage increase
      - distance to nearest uncovered instruction, recency of previous coverage, …
    - **subpath-guided-**/CFG-based branch order (CarFast)
      - favor paths closest to to any uncovered instruction from static CFG (obtained from static analysis)
    - **evolutionary search** of test input space (genetic programming)
      - fitness based on results of static/dynamic analysis
    - **buggy-path first strategy:** prioritize branches which contained more (non-critical) bugs, assumes heterogenous code quality in code base
    - prioritize certain instruction types/control flow patterns such as loops, symbolic addresses, … that are likely to contain errors
    - fitness functions, e.g. difference between variables that should be equal for a condition
  - random testing (test conditions for random inputs)
- caches:
  - cache results (pre- and post-conditions) per function
  - discard execution at paths that were already reached with same constraints
- parallelization (split up search space, run on multiple CPUs/nodes)

### Memory Modeling

problem: symbolic reasoning of, e.g., arrays, memory aliases, arithmetic overflow, pointers/OOPs/function pointers with symbolic memory access (e.g., symbolic pointer, symbolic array index, symbolic object class)

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

approaches:

- **fully symbolic memory:** complete, but possible path explosion/memory overhead

  - **state forking:** fork for all possible states
    - possible path explosion!
  - conditional expressions in symbolic store (comparably with online symbex)
    - possible memory overhead
  - symbolic addresses as keys in symbolic store

  - **heap modeling** of pointers: pointers can only point to known (previously allocated) objects

- **address concretization:** limited completeness, but reasonable performance

  - heuristics: null pointer or new allocations, repeated recursively for fields in referenced data structure
    - possible path explosion: upper bounds/priorities
  - optional **lazy initialization** of fields on read (**generalized symbolic execution**)
  - optional user-provided preconditions/assertions

- **partial** memory model: only concretize some addresses

  - read vs write addresses (read addresses are easier for symbolic memory)
  - size of estimated possible space (needs to be small for symbolic memory)
  - estimation: test random concretizations with solver (expensive), incomplete, further heuristics

- concolic execution: do not invert symbolic pointers, but possibly omit some branches

trade-off based on analysis goals (low-level vs high-level)

- increased model complexity -> increased computation complexity
- controlling: choice of concretized values (for instance, by excluding `MAX_INT`, `nullptr`, etc.)

### Blackbox Environment/Nondeterministic Behavior

Examples: syscalls, concurrency (multithreading, accelerators), frameworks (inversion of control)

**path divergence:** nondeterministic behavior leads to choice of different branch than expected when negating constraints for concolic execution

- rates of >60%

Solutions:

- pass **concrete values** to blackbox without isolation (pollutes environment, unwanted/untracked side effects, limited code coverage)
  - concretized value using solver
  - random value
- **emulation** (complex models): mocks/environment drivers for providing state in symbex and reproducing it in generated tests
  - return independent symbols: even detect attacks from infected environment, faster to build, but possibly too sensitive
- **include environment** in symbex
  - alternative strategies:
    - **virtualize environment** (and fork emulator)
    - **translate** it to symbex engine-compatible representation
  - memory/performance overhead! full implementation contains more aspects than required for covering application.
- heuristic approaches:
  - **sub-callgraphs** and **regular input grammars** (see „Testing Android Apps“)
  - fuzzing
  - program slicing
  - model generation

### Limitations of Constraint Solver

Not efficiently solvable constraint sets (missing theory knowledge / efficient algorithm not known/existing/implemented / blackbox expression)

- examples: hash, prime factorization, …

Solutions:

- combination of different **theories** that are faster for parts of constraint set
- **imprecision**
  - approximated solutions
  - random values
  - may miss some execution paths
- optimizations for brute-forcing
  - **constraint reduction:**
    - simplification/expression rewriting
    - partition independent constraint sets
    - eliminate irrelevant constraints: if only some constraints change (concolic execution), reuse all parts of the previous solution that the changed constraints do not depend on
  - **reuse solutions:** cache and reuse solutions for similar constraint sets (subsets: trivial, supersets: reduce number of unknown variables)
- **lazy constraints:** defer expensive constraint sets and compute them at path completion (probably easier then as more constraints are given)
  - overhead in number of paths, but reduces pressure on SMT solver (useful if solver timeout is chosen well)

Value concretization: prefer simple and human-readable values (e.g., `1` over `4328902461` or `abc` over `°╚Ã`)

### Trends

- SMT solvers
  - faster SAT algorithms and theories
  - constraint caching strategies
  - domain-specific solver theories
- branch prioritization
  - advanced heuristics
  - **probabilistic symbolic execution:** user-specified or mined branch weights
    - also used to predict reliability and performance of application
- concurrency models
- exploitation of static analysis
  - static control-flow graph (CFG) for branch prioritization and path merging
  - type flow information and statically generated value dependencies (e.g., TypeScript) for additional constraints and choice of memory model
  - program slicing, taint analysis, fuzzing, …
- **separation logic** for „checking memory safety property“ in unmanaged code (?)
