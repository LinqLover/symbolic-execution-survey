/* Build:
    $ clang -emit-llvm -c foo.c
    $ klee foo.bc
*/
#include <stdlib.h>
//#include "klee/klee.h"

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
    if (!(x + y + z != 3)) abort();
}

int main() {
    int a, b, c;
    klee_make_symbolic(&a, sizeof(a), "a");
    klee_make_symbolic(&b, sizeof(b), "b");
    klee_make_symbolic(&c, sizeof(c), "c");
    foo(a, b, c);
    return 0;
}
