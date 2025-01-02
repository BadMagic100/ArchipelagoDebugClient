# ArchipelagoDebugClient

A cross-platform debugging client for use by Archipelago developers, written in C# and Avalonia.

## Why?

Archipelago is all about sending and receiving various data over the network. Of course, this means
that it is useful to have something on the other side of the network which can send and receive data
to help test during the development of your own game client. However, for a long time, no publicly
available debug client existed, with the most commonly stated reasons being: 

1. Every developer should be capable of implementing their own client if they want to accomplish anything, and
2. If a debug client exists, it will certainly be used for nefarious purposes, e.g. to ruin public games.

I found neither of these reasons satisfactory (for 1, I agree but "should be able to do it" and "must do it"
are meaningfully different; for 2, abuse prevention measures can be added, more detail below). I resolved to
do it myself the moment I needed it, in the hopes it might also be useful to someone else in the future.
I found that need when implementing gifting for Hollow Knight, so now we have this.

As for why it's done in C# and not in Python/AP Core - mostly it is because I expected resistance from core
along the lines above, which I did not have any interest in fighting. However I also wanted a chance to try 
out Avalonia for something and have a (perhaps unfounded) distaste for Kivy.

## Abuse prevention measures

> If a debug client exists, it will certainly be used for nefarious purposes

This is super fair concern! However, it is not a blocker. The debug client does 2 main things to help in this area:

1. The client will refuse to connect on its privileged connection if there are more than 2 players in the multiworld,
   or if the race mode flag is set in data storage.
2. It can't send goal or any commands, preventing mass collect/release from ruining everyone's day.

I believe these measures are sufficient, as anyone who has the coding ability to change these contstraints to cause
problems would also have no problem to do it with CommonClient without this tool existing.

## Features

### Log

![log page of the debug client](/Screenshots/Log.png)

The log page shows the message log from the server, just as you would see it in the text client.
Sending commands is not currently supported.

### Locations

![location page of the debug client](/Screenshots/Locations.png)

The locations page allows you to send locations from the connected slot. This is useful for testing
receipt of items and for testing the behavior of co-op slots.

### DeathLink

![deathlink page of the debug client](/Screenshots/DeathLink.png)

The DeathLink page allows sending and receiving deathlinks. Perhaps marginally easier than using sudoku!

### Gifting

![gifting page of the debug client](/Screenshots/Gifting.png)

The gifting page allows sending gifts using the [cross-game gifting protocol](https://github.com/agilbert1412/Archipelago.Gifting.Net/blob/main/Documentation/Gifting%20API.md).
Receiving gifts is not yet supported (contributions welcome!)

### DataStorage

![datastorage page of the debug client](/Screenshots/DataStorage.png)

The DataStorage page allows observing and inspecting the values in datastorage as they change. Users can list
available datastorage keys on the server with `/datastore`, then watch keys of their choice.

Known quirk: Watching the `GiftBoxes;{team}` key can sometimes prevent the gifting page from sending gifts
for reasons that are not entirely clear. As such, it is recommended not to watch these keys in the same 
session as you're sending out gifts.

### SlotData

![slotdata page of the debug client](/Screenshots/SlotData.png)

The SlotData page allows inspecting the values in slot data, similar to the DataStorage page.

## How it works

After entering your connecting details, the client will briefly connect to the server as a tracker
to perform sanity checks and infer the desired slot's game. It will then reconnect on that slot
with full permission to send checks/bounce packets/etc.
