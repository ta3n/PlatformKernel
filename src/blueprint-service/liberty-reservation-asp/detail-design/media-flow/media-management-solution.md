# Liberty Media Management With Amazon S3

## Table of Contents

- [Liberty Media Management With Amazon S3](#liberty-media-management-with-amazon-s3)
  - [Table of Contents](#table-of-contents)
  - [1. Current sequence diagram for Media](#1-current-sequence-diagram-for-media)
    - [Upload media sequence diagram](#upload-media-sequence-diagram)
    - [Get media sequence diagram](#get-media-sequence-diagram)
  - [2. Impact of High Concurrent Media Resize Requests on ECS Instance Performance](#2-impact-of-high-concurrent-media-resize-requests-on-ecs-instance-performance)
    - [Scenario](#scenario)
    - [Current Workflow](#current-workflow)
    - [Performance Issues](#performance-issues)
    - [Impact](#impact)
  - [3. Asynchronous Media Resizing Solution using RabbitMQ and Amazon S3 with Original Media Fallback](#3-asynchronous-media-resizing-solution-using-rabbitmq-and-amazon-s3-with-original-media-fallback)
    - [Context and Problem Statement](#context-and-problem-statement)
    - [This workflow results in critical performance issues](#this-workflow-results-in-critical-performance-issues)
    - [Proposed Comprehensive Solution](#proposed-comprehensive-solution)
    - [Solution Overview (Key Points)](#solution-overview-key-points)

## 1. Current sequence diagram for Media

### Upload media sequence diagram

```mermaid
sequenceDiagram
  participant Client as Client
  participant Api as Manager.File.WebApi
  participant Handler as Handler
  participant DB as Database
  participant AmazonS3 as Amazon S3

  Client-->>Api: 1. Upload media request
  Api-->>Handler: 2. Save media
  Handler-->>AmazonS3: 3. Upload media to storage
  AmazonS3-->>Handler: 4. Return media code

  alt Success
    Handler-->>DB: 5. Create media
    DB-->>Handler: 6. Return success result
    Handler-->>Api: 7. Return success response
    Api-->>Client: 8. Return success response
  else Failure
    Handler-->>DB: 5. Create media
    DB-->>Handler: 6. Return error
    Handler-->>AmazonS3: 7. Remove media from storage (rollback)
    AmazonS3-->>Handler: 8. Media removed confirmation
    Handler-->>Api: 9. Return failure response
    Api-->>Client: 10. Return failure response
  end
```

### Get media sequence diagram

```mermaid
sequenceDiagram
  participant Client as Client
  participant Api as Manager.File.WebApi
  participant Handler as Handler
  participant AmazonS3 as Amazon S3

  Client-->>Api: 1. Get media by (code, size) type request
  Api-->>Handler: 2. Get media
  Handler-->>AmazonS3: 3. Get media by size from storage
  AmazonS3-->>Handler: 4. Return media with size request

  alt Media with requested size exists
    Handler-->>Api: 5. Return media response
    Api-->>Client: 6. Return media response
  else Media with requested size not existing
    Handler-->>AmazonS3: 5. Get original media by code
    AmazonS3-->>Handler: 6. Return original media
    Handler-->>Handler: 7. Resize original media to requested size
    Handler-->>AmazonS3: 8. Upload resized media
    AmazonS3-->>Handler: 9. Confirm upload
    Handler-->>Api: 10. Return media response
    Api-->>Client: 11. Return media response
  end
```

## 2. Impact of High Concurrent Media Resize Requests on ECS Instance Performance

```mermaid
sequenceDiagram
  participant Client as Client
  participant Api as Manager.File.WebApi
  participant Handler as Handler
  participant AmazonS3 as Amazon S3
  participant ECS as ECS Instance

  Client-->>Api: 1. Get media by (code, size) type request (50 simultaneous requests)
  Api-->>Handler: 2. Get media
  Handler-->>AmazonS3: 3. Check media by size from storage
  AmazonS3-->>Handler: 4. Media with requested size not found

  loop For each simultaneous request
    Handler-->>AmazonS3: 5. Get original media by code
    AmazonS3-->>Handler: 6. Return original media
    Handler-->>Handler: 7. Resize original media to requested size
    Handler-->>AmazonS3: 8. Upload resized media
    AmazonS3-->>Handler: 9. Confirm upload
  end

  Note over Handler,ECS: High CPU usage due to simultaneous resize operations
  Handler-->>ECS: CPU overload
  ECS-->>ECS: Instance crashes and restarts new instance

  Handler-->>Api: 10. Return media response
  Api-->>Client: 11. Return media response
```

### Scenario

- Approximately 50 simultaneous requests to retrieve media by specific code and requested size.

### Current Workflow

1. Client requests media by (code, size).
2. Handler checks Amazon S3 for the requested resized media.
3. If the resized media doesn’t exist:

    - Handler retrieves the original media from Amazon S3.
    - Performs on-demand resizing directly within the Handler.
    - Uploads resized media back to Amazon S3.

4. Finally, returns the resized media response to the client.

### Performance Issues

- CPU-intensive resizing operations running concurrently cause high CPU utilization.
- Leads to ECS instance overload, resulting in frequent instance crashes.
- ECS must restart or provision new instances, causing downtime and instability.

### Impact

- Reduced service availability.
- Degraded performance and reliability.
- Negative user experience due to delays or service interruption.

## 3. Asynchronous Media Resizing Solution using RabbitMQ and Amazon S3 with Original Media Fallback

### Context and Problem Statement

Given the current scenario where approximately 50 concurrent requests trigger the resizing of non-existent media, the system follows these steps for each request:

  1. Retrieve the original media file from Amazon S3.
  2. Resize the media directly on the ECS instance.
  3. Upload the resized media back to Amazon S3.

### This workflow results in critical performance issues

- High CPU utilization due to intensive, concurrent resizing tasks.
- ECS instance overload, leading to instance crashes and automatic instance recreation.

### Proposed Comprehensive Solution

To thoroughly address and resolve the described issue, the following comprehensive solution is proposed:

```mermaid
sequenceDiagram
  participant Client as Client
  participant Api as Manager.File.WebApi
  participant Handler as Handler
  participant DB as Database
  participant AmazonS3 as Amazon S3
  participant RabbitMQ as RabbitMQ Queue
  participant Worker as ECS Resize Worker
  participant CleanupWorker as ECS Cleanup Worker

  rect rgb(200, 255, 200)
    Note over Client, Handler: [1] Upload media flow
    Client-->>Api: 1. Upload media request
    Api-->>Handler: 2. Save media
    Handler-->>AmazonS3: 3. Upload media to storage
    AmazonS3-->>Handler: 4. Return media code

    alt Success
      Handler-->>DB: 5. Create media record
      DB-->>Handler: 6. Return success result
      Handler-->>RabbitMQ: 7. Publish resize task (media code, predefined sizes)
      Handler-->>Api: 8. Return success response
      Api-->>Client: 9. Return success response
    else Failure
      Handler-->>DB: 5. Create media record
      DB-->>Handler: 6. Return error
      Handler-->>AmazonS3: 7. Remove media from storage (rollback)
      AmazonS3-->>Handler: 8. Media removed confirmation
      Handler-->>Api: 9. Return failure response
      Api-->>Client: 10. Return failure response
    end
  end

  rect rgb(200, 200, 255)
    Note over Worker, AmazonS3: [2] Asynchronous resize operation
    Worker-->>RabbitMQ: 1. Consume resize task
    Worker-->>AmazonS3: 2. Get original media
    AmazonS3-->>Worker: 3. Return original media

    Note over Worker: Resize media to predefined sizes:
    Note over Worker: - Small<br/>- Medium<br/>- Large<br/>- Compress50<br/>- Compress100<br/>- Compress150<br/>- Compress300<br/>- Compress500<br/>- Compress1024

    loop For each predefined size
      Worker-->>Worker: 4. Resize media
      Worker-->>AmazonS3: 5. Upload resized media
      AmazonS3-->>Worker: 6. Confirm upload success
    end
    Worker-->>DB: 7. Update resize status to "Completed"
  end

  rect rgb(255, 250, 200)
    Note over Client, Handler: [3] Get media flow
    Client-->>Api: 1. Get media by (code, size) type request
    Api-->>Handler: 2. Get media
    Handler-->>AmazonS3: 3. Check media by size from storage

    alt Media with requested size exists
      AmazonS3-->>Handler: 4. Return resized media
      Handler-->>Api: 5. Return resized media
      Api-->>Client: 6. Return resized media response
    else Media with requested size not existing
      AmazonS3-->>Handler: 4. Resized media not found
      Handler-->>AmazonS3: 5. Get original media by code
      AmazonS3-->>Handler: 6. Return original media
      Handler-->>Api: 7. Return original media response
      Api-->>Client: 8. Return original media response
    end
  end

  rect rgb(255, 200, 200)
    Note over CleanupWorker, AmazonS3: [4] Cleanup orphaned media operation
    CleanupWorker-->>DB: 1. Query orphaned media records
    DB-->>CleanupWorker: 2. Return orphaned media list

    loop For each orphaned media
      CleanupWorker-->>AmazonS3: 3. Move resized media to Trash folder
      AmazonS3-->>CleanupWorker: 4. Confirm move success
    end
  end
```

### Solution Overview (Key Points)

1. Media Upload Flow:

    - Client uploads original media files via API.
    - Original media is stored on Amazon S3, and metadata saved to the database.
    - **A resize task is published to a RabbitMQ queue upon successful upload.**

2. Asynchronous Resizing:

    - **ECS Resize Worker asynchronously processes media resize tasks from RabbitMQ.**
    - Original media files are resized into predefined sizes.
    - Resized images are stored back on Amazon S3.

3. Media Retrieval with Fallback:

    - When clients request media of specific sizes:
    - If resized media exists, it’s returned immediately.
    - If resized media is unavailable, the original media is returned as a fallback.

4. Cleanup of Orphaned Media:

    - A dedicated Cleanup Worker periodically identifies orphaned media files.
    - Orphaned resized media files (no longer associated with any entity) are moved into a Trash folder on Amazon S3 to optimize storage use.
