from typing import Optional

class Node:
    def __init__(self, value: int, left: Optional['Node']=None, right: Optional['Node']=None):
        self.value = value
        self.left = left
        self.right = right

    def rotate_left(self):
        new_root = self.right
        self.right = new_root.left
        new_root.right = self
        return new_root

    def rotate_right(self):
        new_root = self.left
        self.left = new_root.right
        new_root.right = self
        return new_root

    def insert(self, value):
        if value < self.value:
            if self.left:
                self.left.insert(value)
            else:
                self.left = Node(value, parent=self)
        else:
            if self.right:
                self.right.insert(value)
            else:
                self.right = Node(value, parent=self)

    def find(self, value: int):
        if value < self.value:
            return self.left.find(value) if self.left else None
        elif value > self.value:
            return self.right.find(value) if self.right else None

    def height(self):
        return 1 + max(
            self.left.height() if self.left else 0,
            self.right.height() if self.right else 0
        )

    def count(self):
        return 1 \
            + (self.left.count() if self.left else 0) \
            + (self.right.count() if self.right else 0)

    def __str__(self):
        return f'{self.value}' + f'({self.left or ""},{self.right or ""})' * bool(self.left or self.right)

    def __repr__(self):
        return f"{self.__class__.__name__}({self.value}{f', left={repr(self.left)}' if self.left or self.right else ''}{f', right={repr(self.right)}' if self.right else ''})"
