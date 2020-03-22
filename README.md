
m68k
====

This is "my" Motorola 680x0 emulator written in C#. This is the designed to be a modular cpu component for my C# based Megadrive emulator project [https://github.com/maximilien-noal/YAME](YAME).  This project is forked from https://github.com/fedex81/m68k.

Emulation
---------

Currently m68k emulates the 68000 cpu found in the Amiga and other 16-bit era machines. There is no reason why this couldn't be extended to support the rest of the 680x0 family and this was a consideration of the original design.

Building
--------

```
dotnet build
```

You need a .NET Standard 2.0 compliant implementation in order to use this library.

Running
-------

As with the original Java code, there is a simple cpu monitor shell to enable testing/debugging. This can be invoked by running the following at the command prompt:

	$ dotnet run m68k.Monitor


Feedback, Comments, Bugs etc.
-----------------------------

Please give me your feedback and comments and report any bugs via my GitHub project.


License
-------
The code is released under the Open Source BSD License.


Contributors
------------
Many thanks to [Wolfgang Lenerz](https://github.com/flockermush) and @fedex81 for their contributions.
