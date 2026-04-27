#!/usr/bin/env bash
# test_smoke_dot_bridge_value.sh — live FIFO smoke for S-2-bridge-2.
#
# Validates the assignment chokepoint VALUE fire-point in
# Executive.Assign emits exactly:
#
#   #0  kind=VALUE name_id=0 STRING(5)=b'hello'
#   #1  kind=END   name_id=0xffffffff NULL(empty)
#
# for the canonical hello probe:
#
#       S = 'hello'
#   END
#
# Plus the names sidecar contains exactly "S\n" (no BOM, LF terminator)
# at id=0 — byte-identical to what csn's lvalue_name_id would intern.
#
# Sat Apr 27 2026 — landed with S-2-bridge-2.

set -uo pipefail
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

SNO4_REPO="${SNO4_REPO:-/home/claude/snobol4dotnet}"
SNO4="${SNO4:-$SNO4_REPO/Snobol4/bin/Release/net10.0/Snobol4.dll}"
READER="${READER:-/home/claude/one4all/scripts/monitor/read_one_wire.py}"

if ! command -v dotnet >/dev/null 2>&1; then
    if [ -x /usr/local/dotnet10/dotnet ]; then export PATH="/usr/local/dotnet10:$PATH"
    else echo "SKIP: dotnet missing"; exit 0; fi
fi
if [ ! -f "$SNO4" ]; then echo "SKIP: $SNO4 not built"; exit 0; fi
if [ ! -f "$READER" ]; then echo "SKIP: $READER missing"; exit 0; fi

TD="$(mktemp -d -t dot_bridge_value.XXXXXX)"
trap 'rm -rf "$TD"' EXIT

cat > "$TD/probe.sno" << 'EOF'
            S = 'hello'
END
EOF

python3 "$READER" "$TD/ready.fifo" "$TD/go.fifo" "$TD/names.txt" \
    > "$TD/ctrl.out" 2> "$TD/ctrl.err" &
CTRL=$!

# Brief delay so the controller's mkfifo + open(O_RDONLY) is reached
# before the participant tries to open(O_WRONLY).  FIFO semantics
# require a reader to be present.
sleep 0.5

MONITOR_BIN=1 \
MONITOR_READY_PIPE="$TD/ready.fifo" \
MONITOR_GO_PIPE="$TD/go.fifo" \
MONITOR_NAMES_OUT="$TD/names.txt" \
    timeout 10 dotnet "$SNO4" -b "$TD/probe.sno" > "$TD/dot.out" 2> "$TD/dot.err"
DOT_RC=$?

wait $CTRL 2>/dev/null

PASS=0; FAIL=0

# Check 1: dot exited cleanly
if [ $DOT_RC -eq 0 ]; then PASS=$((PASS+1)); else echo "FAIL dot exit=$DOT_RC"; FAIL=$((FAIL+1)); fi

# Check 2: exactly 2 records
N=$(grep -cE "kind=" "$TD/ctrl.err" || true)
if [ "$N" = "2" ]; then PASS=$((PASS+1)); else echo "FAIL record count: got $N expected 2"; cat "$TD/ctrl.err"; FAIL=$((FAIL+1)); fi

# Check 3: VALUE record carries name_id=0 STRING(5)='hello'
if grep -qE "#0+0 kind=VALUE name_id=0 STRING\(5\)=b['\"]hello['\"]" "$TD/ctrl.err"; then
    PASS=$((PASS+1))
else echo "FAIL VALUE record shape"; cat "$TD/ctrl.err"; FAIL=$((FAIL+1)); fi

# Check 4: END record present
if grep -qE "kind=END" "$TD/ctrl.err"; then
    PASS=$((PASS+1))
else echo "FAIL END record absent"; FAIL=$((FAIL+1)); fi

# Check 5: names sidecar is exactly "S\n" — no BOM, LF terminator
EXPECTED_HEX="53 0a"
ACTUAL_HEX=$(od -An -t x1 "$TD/names.txt" 2>/dev/null | tr -s ' ' | sed 's/^ //' | tr -d '\n' | sed 's/  *$//')
if [ "$ACTUAL_HEX" = "$EXPECTED_HEX" ]; then
    PASS=$((PASS+1))
else echo "FAIL names sidecar hex: got '$ACTUAL_HEX' expected '$EXPECTED_HEX'"; FAIL=$((FAIL+1)); fi

echo "PASS=$PASS FAIL=$FAIL"
[ $FAIL -eq 0 ]
