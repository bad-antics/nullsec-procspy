# NullSec ProcSpy

```
    ███▄    █  █    ██  ██▓     ██▓      ██████ ▓█████  ▄████▄  
    ██ ▀█   █  ██  ▓██▒▓██▒    ▓██▒    ▒██    ▒ ▓█   ▀ ▒██▀ ▀█  
   ▓██  ▀█ ██▒▓██  ▒██░▒██░    ▒██░    ░ ▓██▄   ▒███   ▒▓█    ▄ 
   ▓██▒  ▐▌██▒▓▓█  ░██░▒██░    ▒██░      ▒   ██▒▒▓█  ▄ ▒▓▓▄ ▄██▒
   ▒██░   ▓██░▒▒█████▓ ░██████▒░██████▒▒██████▒▒░▒████▒▒ ▓███▀ ░
   ░ ▒░   ▒ ▒ ░▒▓▒ ▒ ▒ ░ ▒░▓  ░░ ▒░▓  ░▒ ▒▓▒ ▒ ░░░ ▒░ ░░ ░▒ ▒  ░
   ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄
   █░░░░░░░░░░░░░░░░░░ P R O C S P Y ░░░░░░░░░░░░░░░░░░░░░░░░█
   ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
                       bad-antics
```

![Forth](https://img.shields.io/badge/Forth-Stack%20Based-red?style=for-the-badge)
![NullSec](https://img.shields.io/badge/NullSec-Tool-black?style=for-the-badge)

## Overview

Minimal footprint process monitor and analyzer written in Forth. The concatenative nature of Forth allows for an extremely small binary that can be easily embedded or deployed in constrained environments.

## Features

- **Process Enumeration** - List all running processes
- **Memory Mapping** - View process memory regions
- **File Descriptor Tracking** - Monitor open files and sockets
- **Environment Extraction** - Dump process environment variables
- **Thread Analysis** - Examine process threads
- **Injector Support** - Process injection helpers
- **Minimal Footprint** - Entire tool under 10KB

## Requirements

- gforth (GNU Forth) >= 0.7.3
- Linux (uses /proc filesystem)
- Root for full functionality

## Build

```bash
# Run directly
gforth procspy.fs

# Create standalone executable
gforth -e "include procspy.fs" -e "bye"
```

## Usage

```forth
\ Start interactive mode
gforth procspy.fs

\ List processes
list-procs

\ Analyze specific PID
1337 proc-info

\ Memory map
1337 proc-maps

\ File descriptors
1337 proc-fds

\ Environment
1337 proc-env

\ Find process by name
s" nginx" find-proc

\ Monitor process
1337 watch-proc
```

## Commands

| Command | Stack | Description |
|---------|-------|-------------|
| `list-procs` | ( -- ) | List all processes |
| `proc-info` | ( pid -- ) | Show process information |
| `proc-maps` | ( pid -- ) | Show memory mappings |
| `proc-fds` | ( pid -- ) | Show file descriptors |
| `proc-env` | ( pid -- ) | Show environment |
| `find-proc` | ( c-addr u -- ) | Find process by name |
| `watch-proc` | ( pid -- ) | Continuous monitoring |
| `proc-inject` | ( pid -- ) | Prepare injection |

## Architecture

```
┌────────────────────────────────────────────┐
│              ProcSpy Core                   │
├─────────────┬─────────────┬────────────────┤
│   /proc     │   syscalls  │    ptrace      │
│   reader    │   wrapper   │    interface   │
├─────────────┴─────────────┴────────────────┤
│            Stack-Based Engine              │
└────────────────────────────────────────────┘
```

## License

NullSec Proprietary - For authorized security research only

---

[![GitHub](https://img.shields.io/badge/GitHub-bad--antics-181717?style=flat&logo=github&logoColor=white)](https://github.com/bad-antics)
[![Discord](https://img.shields.io/badge/Discord-killers-5865F2?style=flat&logo=discord&logoColor=white)](https://discord.gg/killers)
