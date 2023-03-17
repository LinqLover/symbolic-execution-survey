"""
Pseudo API usage for a symbolic execution framework in Python. Declarative style, obviously not efficient to trace all prior states unconditionally.
"""
import png
import symbex


tree = symbex.execute(
	png.parse,
	args=[symbex.var('bytes')], kwargs={'acceleration': None},
	preconditions=[
		lambda: isinstance(symbex.var('bytes'), bytearray) and len(symbex.var('bytes')) < 1000
	]
)

# generate tests
tests = [f"png.parse('{state.concrete('bytes')}', acceleration=None)" for state in tree.leaves]

# generate input/output table
table = dict(state.concrete(['bytes', state.return_value]) for state in tree.leaves)

# vulnerability detection
exploits = [
	state.concrete('bytes') for path in tree.paths for state in path
	if any(obj.filter > 3 for obj in state.objects if isinstance(obj, png.Parser))
]
