### General Rules

| No | Checked Items                                                                                 | Assessment | Notes | Priority | Severity |
|----|-----------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | The subject of the commit message is limited to 50 characters                                 | Mandatory  |       | 1        |          |
| 2  | Capitalize the first letter of the subject                                                    |            |       | 2        |          |
| 3  | Do not end the subject with a period                                                          |            |       | 2        |          |
| 4  | Use an imperative style in the subject (Add password validation vs Added password validation) |            |       | 2        |          |
| 5  | Add a commit body when additional background for the commit is necessary                      |            |       | 2        |          |
| 6  | Body is separated from subject by one blank line                                              | Mandatory  |       | 1        |          |

### Semantic Subjects

| No | Checked Items                                                                                                                                                                                                                                                                                    | Assessment | Notes | Priority | Severity |
|----|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | Commit messages' subjects are preceded by a tag to make it easier to read through them and filter them out: <br> `<type>: <commit subject>` <br> For example: <br> `feat: #tasknumber short_Description` <br> `fix: #bugNumber short_Description` <br> `hotfix: #hotfixNumber short_Description` | Mandatory  |       | 1        |          |

### Tags

| No | Checked Items                                                                                                | Assessment | Notes | Priority | Severity |
|----|--------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | `feat`: New feature or functionality for the user, not a new feature for the build script                    | Mandatory  |       | 1        |          |
| 2  | `fix`: Bug fix for the user, not a fix to a build script                                                     | Mandatory  |       | 1        |          |
| 3  | `docs`: Changes to the documentation                                                                         |            |       | 2        |          |
| 4  | `style`: Formatting, missing semi-colons, etc; no production code change                                     |            |       | 2        |          |
| 5  | `refactor`: Code refactoring (variable renaming or code restructuring) that doesn't affect the functionality |            |       | 2        |          |
| 6  | `test`: Adding, fixing, or refactoring tests; no production code change                                      |            |       | 3        |          |
| 7  | `chore`: Updating build scripts or upgrading dependencies; no production code change                         |            |       | 3        |          |
| 8  | `misc`: Use for anything that doesn't clearly fall into any of the previous categories                       |            |       | 3        |          |

### Branch Name

| No | Checked Items                                                                                                                                                                                                            | Assessment | Notes | Priority | Severity |
|----|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | Branch name with new features <br> `features/<ticket_number>_<summary_feature>` <br> `bugs/<ticket_number>_<summary_feature>` <br> `hotfixs/<ticket_number>_<summary_feature>` <br> Example: `features/001_login_logout` | Mandatory  |       | 2        |          |

### Pull Request Name

| No | Checked Items                                                                                                                                                    | Assessment | Notes | Priority | Severity |
|----|------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|-------|----------|----------|
| 1  | Pull Request title <br> `[<type>] <PR title>` <br> For example: <br> `[Feat] short_Description` <br> `[Fix] short_Description` <br> `[Hotfix] short_Description` | Mandatory  |       | 1        |          |

---

## Code Review Checklist

### General

| No | Checked Items                                                     | Assessment | Notes | Priority  | Severity |
|----|-------------------------------------------------------------------|------------|-------|-----------|----------|
| 1  | Does the code comply with the language and framework conventions? |            |       | Mandatory | 1        |

### Commenting

| No | Checked Items                                              | Assessment | Notes | Priority  | Severity |
|----|------------------------------------------------------------|------------|-------|-----------|----------|
| 1  | Has the comment been updated with the latest code?         |            |       | Mandatory | 1        |
| 2  | Is the comment clear and correct with the code?            |            |       |           |          |
| 3  | Did the comment describe why and how the code works?       |            |       |           |          |
| 4  | Exceptions, errors around have been commented yet?         |            |       |           |          |
| 5  | Has each operation and feature cluster been commented yet? |            |       |           |          |
| 6  | Have related events and features been commented yet?       |            |       |           |          |
| 7  | Is there a comment on each class title?                    |            |       |           |          |

### Source Code

| No | Checked Items                                                                                                                      | Assessment | Notes | Priority  | Severity |
|----|------------------------------------------------------------------------------------------------------------------------------------|------------|-------|-----------|----------|
| 1  | Does the name of the method/function make sense and describe what it will do?                                                      |            |       | Mandatory | 2        |
| 2  | Are the params used already described?                                                                                             |            |       | Mandatory | 1        |
| 3  | Are normal and exception streams clearly separated?                                                                                |            |       | Mandatory | 1        |
| 4  | When a logic thread is too long, has the action been split into smaller methods?                                                   |            |       |           | 2        |
| 5  | When the logic flow is too long, is it possible to reduce the conditional syntaxes like if-else, while, etc?                       |            |       |           | 2        |
| 6  | Have you minimized nested loops?                                                                                                   |            |       |           | 3        |
| 7  | Is the given variable name meaningful and easy to understand?                                                                      |            |       | Mandatory | 3        |
| 8  | Is the code easy to understand and straight to the point?                                                                          |            |       |           | 3        |
| 9  | With the complex code, are there explanations and comments to avoid confusion when maintaining?                                    |            |       | Mandatory | 2        |
| 10 | Have you used Tab with Space logically according to the structure?                                                                 |            |       |           | 2        |
| 11 | Is there only 1 command per line? Do not put multiple commands in 1 line, because it will be difficult to read and maintain later. |            |       |           | 1        |
| 14 | Is the variable name different from the class name?                                                                                |            |       | Mandatory | 3        |
| 15 | Is the function/method name set according to the general rules? For example, camelCase, is a verb.                                 |            |       |           | 2        |
| 16 | Is the global function's name different from the local function?                                                                   |            |       | Mandatory | 3        |
| 19 | Are names for folders and libraries defined in the document?                                                                       |            |       | Mandatory | 2        |
| 20 | Did the name and type of the folder match the required framework? For example, folder ./src to contain source code.                |            |       | Mandatory | 2        |
| 21 | Are 3rd party libraries used? If yes, then:                                                                                        |            |       |           |          |
| 22 | Has the customer approved this listing?                                                                                            |            |       |           |          |
| 23 | Is the license for the libraries to use appropriate and approved?                                                                  |            |       |           |          |
| 24 | Is there a line of code that is not being used?                                                                                    |            |       |           | 3        |

---

## Coding Guideline

### Get liberty-asp submodules

- init

  ```bash
  git submodule update --init --recursive
  ```

- update

  ```bash
  git submodule update --remote --recursive
  ```

- [liberty-asp](https://github.com/liberty-membership/liberty-asp)

  ```bash
  git submodule add https://github.com/liberty-membership/liberty-asp
  ```

### Migration

```bash
dotnet ef migrations add --project Liberty.Reservation.Employee.WebAPI a001
dotnet ef database update --project Liberty.Reservation.Employee.WebAPI
```

### Code quality

**By Script :**

1. Run Sonar in container : `docker compose -f ./docker/sonar.yml up -d`

2. Wait container was up Run `SonarAnalysis.ps1` or `sonar-analysis.sh` and go to <http://localhost:9001>

**Manually :**

1. Run Sonar in container : `docker compose -f ./docker/sonar.yml up -d`

2. Install sonar scanner for .net :

`dotnet tool install --global dotnet-sonarscanner`

3.

Run `` dotnet sonarscanner begin /d:sonar.login=admin /d:sonar.password=admin /k:"Liberty" /d:sonar.host.url="http://localhost:9001" /s:"`pwd`/SonarQube.Analysis.xml" ``

4. Build your application : `dotnet build`

5. Publish sonar results : `dotnet sonarscanner end /d:sonar.login=admin /d:sonar.password=admin`

6. Go to <http://localhost:9001>
