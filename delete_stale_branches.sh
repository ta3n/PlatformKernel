#!/bin/bash

# Define an array of protected branches
PROTECTED_BRANCHES=(
    "main"
    "prod"
    "stage"
    "develop"
)

# Convert array into regex pattern for grep
PROTECTED_PATTERN="origin/(${PROTECTED_BRANCHES[0]}"
for branch in "${PROTECTED_BRANCHES[@]:1}"; do
    PROTECTED_PATTERN+="|$branch"
done
PROTECTED_PATTERN+="$)$"

echo "📌 Listing branches that will be deleted from Remote:"
git branch -r | grep -vE "$PROTECTED_PATTERN" | sed 's/origin\///'

# Confirm before deleting remote branches
read -p "❗ Are you sure you want to delete these branches from remote? (Y/N): " confirm_remote

if [[ "$confirm_remote" =~ ^[Yy]$ ]]; then
    git branch -r | grep -vE "$PROTECTED_PATTERN" | sed 's/origin\///' | xargs -I {} git push origin --delete {}
    echo "✅ Selected branches have been deleted from Remote."
else
    echo "🚫 Remote branch deletion canceled."
fi

echo "📌 Listing branches that will be deleted from Local:"
git branch | grep -vE "^(${PROTECTED_BRANCHES[0]}"
for branch in "${PROTECTED_BRANCHES[@]:1}"; do
    echo -n "|$branch"
done
echo ")$"

# Confirm before deleting local branches
read -p "❗ Are you sure you want to delete these branches from local? (Y/N): " confirm_local

if [[ "$confirm_local" =~ ^[Yy]$ ]]; then
    git branch | grep -vE "^(${PROTECTED_BRANCHES[0]}"
    for branch in "${PROTECTED_BRANCHES[@]:1}"; do
        echo -n "|$branch"
    done
    echo ")$" | xargs git branch -D
    echo "✅ Selected branches have been deleted from Local."
else
    echo "🚫 Local branch deletion canceled."
fi

echo "🎉 Process completed!"
