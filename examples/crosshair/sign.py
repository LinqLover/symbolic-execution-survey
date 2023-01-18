def sign(x: int):
    """
    post: -1 <= __return__ <= 1
    """
    if x > 0:
        return 1
    else:
        return -1


def test_sign(x: int)
    """
    post: True
    """
    import numpy as np
    assert sign(x) == np.sign(x)
