# Example: CrossHair

## Installation

See <https://marketplace.visualstudio.com/items?itemName=CrossHair.crosshair>.

**Remarks:** Currently, the extension seems not to work fully under Windows.

## Usage

### Generate Tests

Open `foo.py`, then run the `CrossHair: Generate tests to cover the function at the current cursor position` command.

A new terminal will open and display a list of generated tests for pytest.

![CrossHair: Generate tests to cover the function at the current cursor position](./assets/crosshair_cover+l.png#gh-light-mode-only)![CrossHair: Generate tests to cover the function at the current cursor position](./assets/crosshair_cover+d.png#gh-dark-mode-only)

Alter the terminal command by adding `--coverage_type=path` to increase the number of generated tests.

### Contract Programming

Open `binary.py`, then start the background watcher from the status bar (`CH off`).

After a short time, an annotation will appear at the postcondition of the `rotate_left()` function describing that the contract has been violated.

![CrossHair: Contract annotation](./assets/crosshair_contract+l.png#gh-light-mode-only)![CrossHair: Contract annotation](./assets/crosshair_contract+d.png#gh-dark-mode-only)

As you edit and save the file, the annotations will automatically update.

### Compare Implementations

Open `sign.py`, then start the background watcher from the status bar.

After a short time, an annotation will display that the given implementation of `sign()` does not match the behavior of `numpy.sign()` for the case of `0`.
