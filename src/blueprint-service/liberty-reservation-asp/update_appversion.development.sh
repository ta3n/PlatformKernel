#!/bin/bash

# Check if version argument is provided
if [ -z "$1" ]; then
  echo "❌ Please provide a new version, e.g.: ./update_appversion.development.sh 1.2.3 [-y]"
  exit 1
fi

NEW_VERSION="$1"
AUTO_CONFIRM=false

# Optional: check for -y (auto-confirm)
if [ "$2" == "-y" ]; then
  AUTO_CONFIRM=true
fi

# Ignored file paths (relative from script root)
IGNORED_PATHS=(
  "./src/modules/Liberty.Reservation.AppHost/appsettings.Development.json"
)

echo "🔍 Searching for appsettings.Development.json files (excluding bin/, obj/, and ignored files)..."

# Find all appsettings.Development.json files excluding bin/ and obj/
ALL_FILES=$(find ./src -type f -name "appsettings.Development.json" | grep -v "/bin/" | grep -v "/obj/")

# Filter files against ignored paths
FILTERED_FILES=()
while read -r file; do
  SKIP=false
  for ignored in "${IGNORED_PATHS[@]}"; do
    if [[ "$file" == "$ignored" ]]; then
      SKIP=true
      break
    fi
  done
  if [ "$SKIP" = false ]; then
    FILTERED_FILES+=("$file")
  fi
done <<< "$ALL_FILES"

# Check if any file remains
if [ ${#FILTERED_FILES[@]} -eq 0 ]; then
  echo "❌ No valid appsettings.Development.json files found after filtering."
  exit 1
fi

# List files
echo "📂 Files to be updated:"
for f in "${FILTERED_FILES[@]}"; do
  echo "$f"
done

# Ask for confirmation if not auto-confirmed
if [ "$AUTO_CONFIRM" = false ]; then
  echo ""
  read -p "❓ Do you want to update 'AppVersion' to '$NEW_VERSION' in these files? (y/n): " CONFIRM
  if [[ "$CONFIRM" != "y" && "$CONFIRM" != "Y" ]]; then
    echo "❌ Operation cancelled by user."
    exit 0
  fi
fi

# Detect OS type for sed compatibility
OS_TYPE=$(uname)

# Update process
echo ""
echo "🚀 Updating AppVersion..."
echo ""

for file in "${FILTERED_FILES[@]}"; do
  echo "📝 Processing: $file"

  if grep -q '"AppVersion"' "$file"; then
    if [[ "$OS_TYPE" == "Darwin" ]]; then
      # macOS (BSD sed)
      sed -i '' -E "s/(\"AppVersion\"[[:space:]]*:[[:space:]]*\")[^\"]+\"/\1$NEW_VERSION\"/" "$file"
    else
      # Linux, Git Bash (GNU sed)
      sed -i -E "s/(\"AppVersion\"[[:space:]]*:[[:space:]]*\")[^\"]+\"/\1$NEW_VERSION\"/" "$file"
    fi
    echo "✅ Updated to $NEW_VERSION"
  else
    echo "⚠️ Skipped: AppVersion key not found"
  fi
done

echo ""
echo "🎉 Update complete!"
