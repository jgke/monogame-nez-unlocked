MonoGame + Nez with unlocked fps
================================

This repository contains a fork of MonoGame and Nez which has unlocked fps but
with a constant physics step, eg. graphics can run at 200fps but physics will
run at constant 70fps.

Instead of having a single `Update(GameTime)` function for every kinds of
updates, this project has two: `FixedUpdate(GameTime)` for physics updates and
`DrawUpdate(GameTime)` for graphics updates. `GameTime` has a new property
`Alpha` which is a variable with range 0..1 which tells where between two
physics frames the graphics frame is.

Every entity you use needs three positional properties:
- previous physics position
- current physics position
- current graphics position = previous + (current - previous) * alpha

Nez is patched in this repo to update these automatically in FixedUpdate and DrawUpdate:

```c#
public class Entity {
    public virtual void FixedUpdate() {
        PreviousTransform.CopyFrom(Transform);
        Components.FixedUpdate();
    }

    public virtual void DrawUpdate() {
        GraphicsTransform.SetWithLerp(PreviousTransform, Transform, Time.Alpha);
        Components.DrawUpdate();
    }
}
```

Requirements
------------

- .net 5.0
- python3 for tests

Usage
-----

```
$ git clone --recursive https://github.com/jgke/monogame-nez-unlocked
$ make run
```

Updating assets
----------------
- 1) update assets
- 2a) if on windows & visual studio: do nothing
- 2b) if on anything else: `cd GameImpl && ~/.dotnet/tools/t4 ContentPaths.tt -o ContentPaths.cs`



Testing
-------

Run once:
```
$ make env # Needed just once
```

Then:
```
$ source env/bin/activate
$ make test # Run tests
$ make htmltest # Convert test cverage to HTML
$ make stryker # Get test coverage
```

License
-------

MIT, see LICENSE
