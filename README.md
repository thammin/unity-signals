# Unity Signals

A minimal state reactivity library for Unity.

The algorithm underneath is based on [reactively](https://github.com/milomg/reactively).

# Install

via Package Manager UI

```
ssh://git@github.com/thammin/unity-signals.git
```

via [OpenUPM](https://openupm.com/packages/com.thammin.unity-signals/)

```
openupm add com.thammin.unity-signals
```

# Usage

## Signal

A reactive object which you can set or get the value.

```c#
using static UnitySignals.Static;

var source = Signal(1);

Debug.Log(source.Value); // Output: 1
source.Value = 5;
Debug.Log(source.Value); // Output: 5
```

## Camputed

A readonly reactive object which return value from another reactive object.

It will cache the computed value until the dependencies changed.

```c#
using static UnitySignals.Static;

var lastName = Signal("Young");
var firstName = Signal("Paul");
var callCount = 0;

var fullName = Computed(() => 
{
    callCount++;
    return firstName.Value + lastName.Value;
});

Debug.Log(fullName.Value); // Output: "PaulYoung"
Debug.Log(callCount); // Output: 1

// Access the value will return cached value without re-evaluate the getter function
Debug.Log(fullName.Value); // Output: "PaulYoung"
Debug.Log(callCount); // Output: 1

// The internal state of 'fullName' will be marked as dirty
firstName.Value = "Paul's";
Debug.Log(callCount); // Output: 1

// If we try to access the value again will re-evaluate the getter funcion
Debug.Log(fullName.Value); // Output "Paul'sYoung"
Debug.Log(callCount); // Output: 2

```

## Effect

Runs a function immediately while reactively tracking its dependencies.

```c#
using static UnitySignals.Static;

var clickedCount = Signal(0);

var effect = Effect(() => Debug.Log($"ClickedCount={clickedCount}")); // Output: "ClickedCount=0"

clickedCount++; // Output: "ClickedCount=1"

effect.Dispose(); // Dispose the effect to stop the tracking

```

## Watch

Watches reactive object and invokes a callback function that return the old value and the new value.

```c#
using static UnitySignals.Static;

var userCount = Signal(1000);

var watcher = Watch(() => userCount.Value, (newValue, oldValue) =>
{
    Debug.Log($"{oldValue} -> {newValue}");
});

userCount.Value = 3500; // Output: "1000 -> 3500"

watcher.Dispose(); // Dispose the watcher to stop the tracking
```

# References

* https://github.com/milomg/reactively
* https://github.com/tc39/proposal-signals
* https://vuejs.org/api/reactivity-core.html

# License

MIT