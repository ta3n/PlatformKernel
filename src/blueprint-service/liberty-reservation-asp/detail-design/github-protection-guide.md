# 🔐 GitHub Branch & Push Protection Guide

## 🎯 Purpose

This document outlines tools and configurations to protect source code in GitHub by:

- Preventing direct push into protected branches
- Requiring Pull Request (PR) approvals before merging
- Ensuring GitHub Actions pass successfully
- Maintaining a clean and synchronized commit history

## 📌 Scope: `develop`, `stage`, `prod` branches

## 🛠️ Deployment tool

| **Criteria**                | **Branch Protection Rules**                   | **Rulesets**                                                           |
|-----------------------------|-----------------------------------------------|------------------------------------------------------------------------|
| Introduced Since            | Traditional, long-standing                    | Newer feature (2022+)                                                  |
| Scope of Application        | Applies to branches only                      | Applies to branches, tags, and entire repositories                     |
| Management Interface        | `/settings/branches` (per branch)             | `/settings/rules` tab (centralized)                                    |
| Condition Flexibility       | Limited (branch name patterns only)           | Flexible (regex, org-wide, repo-wide, team-based)                      |
| Tag Protection Support      | Not supported                                 | Supported                                                              |
| Condition Configuration     | Fixed, cannot combine                         | Combine multiple conditions in one ruleset                             |
| Protects PRs and Pushes     | Yes                                           | Yes                                                                    |
| GraphQL API Integration     | Partial                                       | Full support                                                           |
| Rule Reusability            | No (per branch setup required)                | Yes (reusable across branches/tags/repos)                              |
| Detail Level                | Basic, easy to understand                     | Advanced, more powerful                                                |
| Recommended For             | Small teams, single repos                     | Larger teams, orgs needing policy standardization                      |

### Should be deployed with Rulesets

---

## ✅ Solution 1: Branch Protection Rules

### 🔧 Configuration Steps

1. Go to: `[repository]/settings/branch_protection_rules/new`
2. Set `Branch name pattern`:

   ```target
   develop
   stage
   prod
   ```

---

### 🔹 develop branch

- ✅ Require a pull request before merging
  - ✅ Require approvals: `1`
  - ✅ Dismiss stale pull request approvals when new commits are pushed
  - ✅ Require approval of the most recent reviewable push
  - ✅ Allow specified actors to bypass required pull requests: `liberty-ishihara` (Optional)
  - ✅ Restrict who can dismiss pull request reviews: `liberty-ishihara`

- ✅ Require status checks to pass before merging
- ✅ Require conversation resolution before merging
- ✅ Lock branch
- ✅ Do not allow bypassing the above settings (Optional)
- ✅ Restrict who can push to matching branches:

  ```text
  yopaz-liberty
  liberty-system
  liberty-ishihara
  ```

---

### 🔹 stage branch

(Same configuration as `develop`)

---

### 🔹 prod branch

(Similar configuration with the following differences)

- ✅ Require approvals: `2`
- ✅ All other rules as per `develop` and `stage`

---

## ✅ Solution 2: Rulesets Protection

### 🔹 Branch Ruleset

1. Go to: `[repository]/settings/rules/new?target=branch`
2. Configuration:
   - **Ruleset Name**: `Protect branches`
   - **Targets**: Include by pattern

     ```text
     develop
     stage
     prod
     ```

   - **Branch Rules**:
     - ✅ Restrict updates
     - ✅ Restrict deletions
     - ✅ Require a pull request before merging
       - Required approvals: `1–2`
       - ✅ Dismiss stale PR approvals
       - ✅ Require approval of latest reviewable push
       - ✅ Require conversation resolution
     - ✅ Require status checks to pass
       - ✅ Require branches to be up to date before merging
       - ✅ Required checks:

         ```text
         FE: Line
         BE: .NET 🧪 Build & Test
         ```

---

### 🔹 Push Ruleset

1. Go to: `[repository]/settings/rules/new?target=push`
2. Configuration:
   - **Ruleset Name**: `Protect push`
   - **Push Rules**:
     - ✅ Restrict file paths:

       ```text
       /.github/workflows/*
       /scripts/*
       buildspec.yml
       .editorconfig
       .gitignore
       Directory.Build.props
       ```

     - ✅ Restrict file extensions:

       ```text
       .exe
       .dll
       .bin
       .zip
       .p12
       .pem
       ```

---

### 📚 References

- [About protected branches](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches)
- [Creating branch protection rules](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches)
- [About rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets)
- [Creating rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/creating-rulesets-for-a-repository)
