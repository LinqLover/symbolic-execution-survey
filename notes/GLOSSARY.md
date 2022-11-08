# Glossary

## Abstract interpretation

## Concolic Testing

## Constraint solver

## CUTE, A concolic unit testing engine for C

## DART, Directed Automated Random Testing

## Dynamic analysis, dynamic program analysis

## Dynamic recompilation

## Environment

## Execution Tree

Nodes are branch statements, edges are branches, labeled with executed statements up to next branch

## Execution-Generated Testing, EGT

## Godzilla

## KLEE

## Memory alias

## Path explosion

## Program path, execution path

Path in execution tree, represents equivalence class of inputs

## SMT solver, SAT solver

Decide whether an equation system/formula can be solved and provide a concrete solution is possible. SAT = satisfiability; SMT = satisfiability modulo theory, also knows axioms for common data types

Operating principle:

- Applies transformations (**tactics**) to simplify/normalize formula
  - examples: constant folding ($x + 0 \to x$), normalize bounds ($k \leq x \to 0 \leq x - k$), bit blasting (split up variable into bits)
- Evaluate heuristics (**predicates** on formula expression) to select tactics
  - handcrafted or learned

Algoritms:

- **DPLL algorithm** for solving boolean expressions in CNF
  - backtracking – divide and conquer over set of variables
  - identify and solve **unit clauses** that only contain single unknown literal (solvable in in constant time), backtrack if mismatch

## Symbolic execution, symbolic evaluation, symbex

## Symbolic executor

## Veritesting

