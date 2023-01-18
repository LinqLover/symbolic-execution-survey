# Example: KLEE

## Installation

See <http://klee.github.io/getting-started/>.

## Usage

Compile sources & run klee:

```sh
$ clang -emit-llvm -c foo.c
$ klee foo.bc
```

Explore the results:

```sh
$ ls klee-last
$ cat klee-last/test00000*.abort.err
$ ktest-tool klee-last/test000001.ktest
```

## Example

See `typescript` (`less -R typescript`).
