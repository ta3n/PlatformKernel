#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

DATE="$(date +%F)"
REPORT="dependency-audit-${DATE}.md"
SLN="Liberty.Reservation.sln"

# Temp files
TMP_DIR="$(mktemp -d)"
VULN_JSON="$TMP_DIR/vulnerable.json"
OUT_JSON="$TMP_DIR/outdated.json"

cleanup() {
  rm -rf "$TMP_DIR" || true
}
trap cleanup EXIT

echo "[dependency-audit] Running dotnet list (vulnerable)..."
dotnet list "$SLN" package --vulnerable --include-transitive --format json > "$VULN_JSON"

echo "[dependency-audit] Running dotnet list (outdated)..."
dotnet list "$SLN" package --outdated --include-transitive --format json > "$OUT_JSON"

# Build the auditor tool
echo "[dependency-audit] Building DependencyAudit tool..."
dotnet build "dependency-audit/DependencyAudit.csproj" -nologo -clp:NoSummary --configuration Release >/dev/null

AUDITOR_DLL="dependency-audit/bin/Release/net8.0/DependencyAudit.dll"

if [ ! -f "$AUDITOR_DLL" ]; then
  echo "[dependency-audit] ERROR: Auditor tool not built at $AUDITOR_DLL" >&2
  exit 1
fi

echo "[dependency-audit] Generating report $REPORT ..."
dotnet "$AUDITOR_DLL" \
  --vulnerable-json "$VULN_JSON" \
  --outdated-json "$OUT_JSON" \
  --solution "$SLN" \
  --output "$REPORT"

echo "[dependency-audit] Done. Report: $REPORT"
