## Wiki Introduction

This Wiki aims to systematically organize the various data resources of SPT, providing modding interface documentation and item creation guides, among other information, to assist developers and players in using this mod and extending SPT items more efficiently.

## Creation Process

After the server starts, Ctrl + left-click on `https://127.0.0.1:6969/SIC` or directly enter this address in your browser.

Then you will see a currently rather simple UI interface.

Click on the navigation bar at the top to enter the item creation interface (item editing is not yet supported, and there is no dedicated ammunition editing page provided for now, but it can still be created through the general item creation page using PropertyOverride).

**All items are disabled by default**, so if you don't see them in the game after saving, this option is usually the issue.

> For subsequent modifications, refer to:
>
> - [Manual Item Creation Method.md](Creation Tutorials/Manual Item Creation Method.md)
> - [0. Understanding Mod Data Structure (Optional).md](Creation Tutorials/0. Understanding Mod Data Structure (Optional).md)
> - [1. Fields Used by the Mod.md](Creation Tutorials/1. Fields Used by the Mod.md)

## Reference Materials

Some of the code, data, and translations mentioned in this Wiki are excerpted from:

> [server-csharp/Libraries/SPTarkov.Server.Assets/SPT_Data/database](https://github.com/sp-tarkov/server-csharp/tree/main/Libraries/SPTarkov.Server.Assets/SPT_Data/database)

Some of the constants mentioned in this Wiki are excerpted from:

> [server-csharp/Libraries/SPTarkov.Server.Core](https://github.com/sp-tarkov/server-csharp/tree/main/Libraries/SPTarkov.Server.Core)

The remaining code and constants are excerpted from this mod.