# Liberty Upload File to Amazon S3

## Overview

This document outlines the process of uploading a file to Amazon S3.

## Sequence Diagram: Upload File to Amazon S3

**The sequence diagram of the upload process**:

```mermaid
sequenceDiagram
    participant User as User
    participant App as File API
    participant S3 as Amazon S3

    User->>App: 1. Upload
    App->>S3: 2. Upload File
    S3-->>App: 3.Upload Success
    App-->>User: 4. FileCode
```
