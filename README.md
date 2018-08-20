# Beat Saber Local Leaderboards Watcher

This is a Windows Service intended to watch the local leaderboard files for Beat Saber and post the contents to a URL when they change.

## Install

Install the Windows service

```cmd
./WindowsService.exe --install
```

## Configuration

Modify `WindowsService.exe.config`:

```xml
<add key="PathToWatch" value="Path\\To\\Leaderboards\\Directory"/>
<add key="PathFilter" value="LocalLeaderboards.dat"/>
<add key="SyncUrl" value="path/to/backend"/>
```

## Running

Start `services.msc` and find the service "Beat Saber Leaderboard Watcher".

Right+Click and select properties.

Configure the service to run with a user with the necessary permissions

"Start service"