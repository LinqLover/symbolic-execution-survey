def foo(a: int, b: int, c: int):
    x = 0
    y = 0
    z = 0
    if a:
        x = -2
    if b < 0:
        if not a and c:
            y = 1
        z = 2
    s = x + y + z
    assert s != 3
    return s
