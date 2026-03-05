# Code Review Perspective

## General

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

## Source Code

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

## Description

- ...

## Checklist

- [ ] Capture the result of the solution not warning: dotnet restore ; dotnet build
- [ ] Capture scan sonarqube with Reliability is 0
- [ ] Capture integration test passed

## Note for Reviewers

- ...
