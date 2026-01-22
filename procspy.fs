\ NullSec ProcSpy - Process Monitor
\ Language: Forth
\ Author: bad-antics
\ License: NullSec Proprietary

\ ============================================================================
\ Banner and Version
\ ============================================================================

: banner ( -- )
  cr
  ."     ███▄    █  █    ██  ██▓     ██▓      ██████ ▓█████  ▄████▄  " cr
  ."     ██ ▀█   █  ██  ▓██▒▓██▒    ▓██▒    ▒██    ▒ ▓█   ▀ ▒██▀ ▀█  " cr
  ."    ▓██  ▀█ ██▒▓██  ▒██░▒██░    ▒██░    ░ ▓██▄   ▒███   ▒▓█    ▄ " cr
  ."    ▓██▒  ▐▌██▒▓▓█  ░██░▒██░    ▒██░      ▒   ██▒▒▓█  ▄ ▒▓▓▄ ▄██▒" cr
  ."    ▒██░   ▓██░▒▒█████▓ ░██████▒░██████▒▒██████▒▒░▒████▒▒ ▓███▀ ░" cr
  ."    ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄" cr
  ."    █░░░░░░░░░░░░░░░░░░ P R O C S P Y ░░░░░░░░░░░░░░░░░░░░░░░░█" cr
  ."    ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀" cr
  ."                        bad-antics v1.0.0" cr cr
;

\ ============================================================================
\ Constants and Variables
\ ============================================================================

256 constant MAX-PATH
1024 constant BUFFER-SIZE
4096 constant LINE-BUFFER-SIZE

variable file-handle
create path-buffer MAX-PATH allot
create line-buffer LINE-BUFFER-SIZE allot
create data-buffer BUFFER-SIZE allot

\ ============================================================================
\ String Helpers
\ ============================================================================

: s+ ( c-addr1 u1 c-addr2 u2 -- c-addr3 u3 )
  \ Concatenate two strings into pad
  >r >r
  dup >r
  pad swap move
  r> dup pad + 
  r> r> rot swap move
  pad swap +
;

: number>string ( n -- c-addr u )
  dup 0< if negate s" -" else s" " then
  rot abs
  0 <# #s #> 
  s+
;

: pid>path ( pid c-addr u -- c-addr' u' )
  \ Build /proc/PID/suffix path
  s" /proc/" 
  rot number>string s+ 
  s" /" s+
  s+
  path-buffer swap 2dup >r >r move r> r>
  path-buffer swap
;

\ ============================================================================
\ File Operations
\ ============================================================================

: file-exists? ( c-addr u -- flag )
  r/o open-file
  if drop false
  else close-file drop true
  then
;

: read-file-line ( -- c-addr u flag )
  \ Read a line from file-handle
  line-buffer LINE-BUFFER-SIZE file-handle @ read-line
  if 2drop line-buffer 0 false
  else line-buffer -rot swap
  then
;

: open-proc-file ( pid c-addr u -- flag )
  pid>path
  r/o open-file
  if drop false
  else file-handle ! true
  then
;

: close-proc-file ( -- )
  file-handle @ ?dup if close-file drop then
;

\ ============================================================================
\ Process Status Parser
\ ============================================================================

: parse-status-line ( c-addr u -- )
  \ Parse and display a status line
  2dup s" Name:" search if
    ."   Name:      " 5 /string type cr
  else 2drop then
  
  2dup s" State:" search if
    ."   State:     " 6 /string type cr
  else 2drop then
  
  2dup s" Pid:" search if
    ."   PID:       " 4 /string type cr
  else 2drop then
  
  2dup s" PPid:" search if
    ."   Parent:    " 5 /string type cr
  else 2drop then
  
  2dup s" Uid:" search if
    ."   UID:       " 4 /string type cr
  else 2drop then
  
  2dup s" VmSize:" search if
    ."   VM Size:   " 7 /string type cr
  else 2drop then
  
  2dup s" VmRSS:" search if
    ."   RSS:       " 6 /string type cr
  else 2drop then
  
  2dup s" Threads:" search if
    ."   Threads:   " 8 /string type cr
  else 2drop then
  
  2drop
;

\ ============================================================================
\ Core Process Functions
\ ============================================================================

: proc-exists? ( pid -- flag )
  s" status" open-proc-file
  dup if close-proc-file then
;

: proc-info ( pid -- )
  cr ." [*] Process Information for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  dup s" status" open-proc-file
  if
    begin
      read-file-line
    while
      parse-status-line
    repeat
    2drop
    close-proc-file
  else
    ." [!] Cannot read process status" cr
    drop
  then
;

: proc-maps ( pid -- )
  cr ." [*] Memory Mappings for PID: " dup . cr
  ." ─────────────────────────────────────────────────────────────────" cr
  ." Address Range              Perms   Offset   Device   Inode  Path" cr
  ." ─────────────────────────────────────────────────────────────────" cr
  
  dup s" maps" open-proc-file
  if
    begin
      read-file-line
    while
      ."   " type cr
    repeat
    2drop
    close-proc-file
  else
    ." [!] Cannot read memory maps" cr
    drop
  then
;

: proc-fds ( pid -- )
  cr ." [*] File Descriptors for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  \ In full implementation, would iterate /proc/PID/fd/
  dup s" fdinfo/0" open-proc-file
  if
    ." FD 0: stdin" cr
    close-proc-file
  else drop then
  
  dup s" fdinfo/1" open-proc-file
  if
    ." FD 1: stdout" cr
    close-proc-file
  else drop then
  
  dup s" fdinfo/2" open-proc-file
  if
    ." FD 2: stderr" cr
    close-proc-file
  else drop then
  
  drop
  ." [*] (Full FD enumeration requires directory iteration)" cr
;

: proc-env ( pid -- )
  cr ." [*] Environment for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  dup s" environ" open-proc-file
  if
    data-buffer BUFFER-SIZE file-handle @ read-file
    if 
      ." [!] Error reading environ" cr
    else
      \ Environment is null-separated
      data-buffer swap
      0 do
        dup i + c@
        dup 0= if
          drop cr
        else
          emit
        then
      loop
      drop
    then
    close-proc-file
  else
    ." [!] Cannot read environment" cr
    drop
  then
;

: proc-cmdline ( pid -- )
  cr ." [*] Command Line for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  dup s" cmdline" open-proc-file
  if
    data-buffer BUFFER-SIZE file-handle @ read-file
    if 
      ." [!] Error reading cmdline" cr
    else
      ." Command: "
      data-buffer swap
      0 do
        dup i + c@
        dup 0= if
          drop space
        else
          emit
        then
      loop
      drop cr
    then
    close-proc-file
  else
    ." [!] Cannot read cmdline" cr
    drop
  then
;

: proc-exe ( pid -- )
  cr ." [*] Executable for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  \ Would use readlink on /proc/PID/exe
  ." [*] Executable path: /proc/" . ." /exe" cr
  ." [*] (Full resolution requires readlink syscall)" cr
;

\ ============================================================================
\ Process Enumeration
\ ============================================================================

: is-digit? ( c -- flag )
  [char] 0 [char] 9 1+ within
;

: is-pid-dir? ( c-addr u -- flag )
  \ Check if all characters are digits (PID directory)
  dup 0= if 2drop false exit then
  true -rot
  0 do
    dup i + c@ is-digit? not if
      rot drop false -rot leave
    then
  loop
  drop
;

: list-procs ( -- )
  cr ." [*] Running Processes" cr
  ." ─────────────────────────────────────────────────────" cr
  ." PID      Name                 State    Memory" cr
  ." ─────────────────────────────────────────────────────" cr
  
  \ Would iterate /proc directory
  \ For demonstration, show current process
  
  ." [*] Process enumeration requires directory iteration" cr
  ." [*] Showing self (PID " pid . ." )" cr
  pid proc-info
;

: find-proc ( c-addr u -- )
  \ Find process by name
  cr ." [*] Searching for process: " 2dup type cr
  ." ─────────────────────────────────────" cr
  
  \ Would iterate /proc and match against /proc/PID/comm
  ." [*] Search requires directory iteration" cr
  ." [*] Pattern stored for matching" cr
  2drop
;

\ ============================================================================
\ Process Monitoring
\ ============================================================================

variable watch-running
variable watch-interval

1000 watch-interval !

: watch-proc ( pid -- )
  cr ." [*] Watching process: " dup . cr
  ." [*] Press Ctrl+C to stop" cr
  ." ─────────────────────────────────────" cr
  
  true watch-running !
  
  begin
    watch-running @
  while
    dup proc-exists?
    if
      ." [" time&date drop drop drop drop . ." :" . ." ] "
      dup s" stat" open-proc-file
      if
        read-file-line
        if type cr else 2drop then
        close-proc-file
      else
        ." Process check OK" cr
      then
      watch-interval @ ms
    else
      ." [!] Process terminated" cr
      false watch-running !
    then
  repeat
  drop
;

\ ============================================================================
\ Injection Support
\ ============================================================================

: proc-inject ( pid -- )
  cr ." [*] Injection Preparation for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  ." [*] Step 1: Attach with ptrace" cr
  ." [*] Step 2: Find suitable injection point" cr
  ." [*] Step 3: Backup original memory" cr
  ." [*] Step 4: Write payload" cr
  ." [*] Step 5: Redirect execution" cr
  ." [*] Step 6: Restore and detach" cr
  cr
  
  dup proc-maps
  
  ." [!] Full injection requires ptrace syscalls" cr
  drop
;

: shellcode-regions ( pid -- )
  cr ." [*] Executable Regions for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  dup s" maps" open-proc-file
  if
    begin
      read-file-line
    while
      \ Look for executable regions (r-xp or rwxp)
      2dup s" r-xp" search if
        ."   [EXEC] " type cr
      else 
        2dup s" rwxp" search if
          ."   [RWX!] " type cr
        else
          2drop
        then
      then
    repeat
    2drop
    close-proc-file
  else
    ." [!] Cannot read maps" cr
    drop
  then
;

\ ============================================================================
\ Security Analysis
\ ============================================================================

: proc-security ( pid -- )
  cr ." [*] Security Analysis for PID: " dup . cr
  ." ─────────────────────────────────────" cr
  
  ." [*] Checking process protections..." cr
  
  \ Check for RWX regions
  ." [*] RWX Regions: "
  dup s" maps" open-proc-file
  if
    0 >r
    begin
      read-file-line
    while
      s" rwxp" search if
        r> 1+ >r
      then
    repeat
    2drop
    close-proc-file
    r>
    ?dup if
      . ." found (potential vulnerability)" cr
    else
      ." none (good)" cr
    then
  else
    ." unknown" cr drop
  then
  
  \ Additional checks would include:
  ." [*] ASLR: Check /proc/sys/kernel/randomize_va_space" cr
  ." [*] NX: Check via /proc/PID/maps permissions" cr
  ." [*] Stack Canary: Check ELF for __stack_chk_fail" cr
  
  drop
;

\ ============================================================================
\ Usage and Help
\ ============================================================================

: usage ( -- )
  cr
  ." Usage:" cr
  ."   list-procs        - List all running processes" cr
  ."   <pid> proc-info   - Show process information" cr
  ."   <pid> proc-maps   - Show memory mappings" cr
  ."   <pid> proc-fds    - Show file descriptors" cr
  ."   <pid> proc-env    - Show environment variables" cr
  ."   <pid> proc-cmdline - Show command line" cr
  ."   <pid> proc-exe    - Show executable path" cr
  ."   s\" name\" find-proc - Find process by name" cr
  ."   <pid> watch-proc  - Monitor process" cr
  ."   <pid> proc-inject - Injection preparation" cr
  ."   <pid> proc-security - Security analysis" cr
  ."   <pid> shellcode-regions - Find executable regions" cr
  cr
  ." Examples:" cr
  ."   1 proc-info" cr
  ."   $$ proc-maps        ( $$ is current shell PID )" cr
  ."   s\" nginx\" find-proc" cr
  cr
;

: help usage ;

\ ============================================================================
\ Main Entry Point
\ ============================================================================

: main ( -- )
  banner
  ." Type 'help' or 'usage' for available commands" cr
  ." Type 'bye' to exit" cr
  cr
;

\ Auto-run banner on load
main
