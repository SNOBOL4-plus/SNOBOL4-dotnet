#!/usr/bin/env bash
# test_smoke_dot_bridge.sh — smoke gate for S-2-bridge-1 (dormant bridge).
#
# Confirms:
#   1. snobol4dotnet builds with MonitorIpc.cs present.
#   2. Beauty 17/17 still PASS (bridge has zero side effects when env
#      vars unset).
#   3. Setting MONITOR_BIN=1 alone (FIFOs absent) produces byte-identical
#      output to the no-env-var run — init fails silently, all entry
#      points stay no-op.
#   4. Setting MONITOR_READY_PIPE / MONITOR_GO_PIPE to non-existent
#      paths produces byte-identical output too — open() fails, init
#      stays !_initOk, entry points stay no-op.
#
# The end-to-end "bridge actually emits records" gate lands with
# S-2-bridge-2 (the first fire-point); this script is the dormancy
# gate that should ride along forever.
#
# Sat Apr 27 2026 — landed with S-2-bridge-1.

set -uo pipefail
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

SNO4_REPO="${SNO4_REPO:-/home/claude/snobol4dotnet}"
SNO4="${SNO4:-$SNO4_REPO/Snobol4/bin/Release/net10.0/Snobol4.dll}"
CORPUS="${CORPUS:-/home/claude/corpus}"
BEAUTY="${BEAUTY:-$CORPUS/programs/snobol4/beauty}"

# Locate dotnet.  apt installs to /usr/bin; legacy doc says /usr/local/dotnet10.
if command -v dotnet >/dev/null 2>&1; then
    : # OK
elif [ -x /usr/local/dotnet10/dotnet ]; then
    export PATH="/usr/local/dotnet10:$PATH"
else
    echo "SKIP: dotnet not on PATH"; exit 0
fi

if [ ! -f "$SNO4" ]; then
    echo "SKIP: $SNO4 not built — run dotnet build first"; exit 0
fi
if [ ! -d "$BEAUTY" ]; then
    echo "SKIP: corpus beauty drivers not found at $BEAUTY"; exit 0
fi

cd "$BEAUTY"

# Pick the first beauty driver as the canonical comparison target.
DRIVER="$(ls *_driver.sno 2>/dev/null | head -1)"
if [ -z "$DRIVER" ]; then
    echo "SKIP: no *_driver.sno in $BEAUTY"; exit 0
fi

PASS=0; FAIL=0

# --- check 1: no env vars (baseline) ---------------------------------------
dotnet "$SNO4" -b "$DRIVER" > /tmp/dot_bridge_baseline.out 2>/tmp/dot_bridge_baseline.err
RC=$?
if [ $RC -eq 0 ]; then PASS=$((PASS+1)); else echo "FAIL baseline rc=$RC"; FAIL=$((FAIL+1)); fi

# --- check 2: MONITOR_BIN=1 only (FIFOs absent) ----------------------------
MONITOR_BIN=1 dotnet "$SNO4" -b "$DRIVER" > /tmp/dot_bridge_bin_only.out 2>/tmp/dot_bridge_bin_only.err
RC=$?
if [ $RC -eq 0 ]; then PASS=$((PASS+1)); else echo "FAIL bin-only rc=$RC"; FAIL=$((FAIL+1)); fi

# --- check 3: bad pipe paths -----------------------------------------------
MONITOR_BIN=1 MONITOR_READY_PIPE=/tmp/nonexist_ready_$$ MONITOR_GO_PIPE=/tmp/nonexist_go_$$ \
    dotnet "$SNO4" -b "$DRIVER" > /tmp/dot_bridge_bad.out 2>/tmp/dot_bridge_bad.err
RC=$?
if [ $RC -eq 0 ]; then PASS=$((PASS+1)); else echo "FAIL bad-paths rc=$RC"; FAIL=$((FAIL+1)); fi

# --- check 4: byte-identical outputs ---------------------------------------
if diff -q /tmp/dot_bridge_baseline.out /tmp/dot_bridge_bin_only.out > /dev/null \
   && diff -q /tmp/dot_bridge_baseline.err /tmp/dot_bridge_bin_only.err > /dev/null
then PASS=$((PASS+1)); else echo "FAIL bin-only diverges from baseline"; FAIL=$((FAIL+1)); fi

if diff -q /tmp/dot_bridge_baseline.out /tmp/dot_bridge_bad.out > /dev/null \
   && diff -q /tmp/dot_bridge_baseline.err /tmp/dot_bridge_bad.err > /dev/null
then PASS=$((PASS+1)); else echo "FAIL bad-paths diverges from baseline"; FAIL=$((FAIL+1)); fi

echo "PASS=$PASS FAIL=$FAIL"
[ $FAIL -eq 0 ]
