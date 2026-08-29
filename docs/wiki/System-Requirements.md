# System Requirements

## Bridge host

- current macOS or Windows with Docker Desktop 4.30+; or
- 64-bit Linux `amd64` or `arm64` with Docker Engine 26+ and Compose v2;
- approximately 100 MB for Bridge and a small persistent state volume;
- enough free disk for container updates and temporary bounded relays;
- reliable system time and DNS.

Bridge does not need a separate database, Kubernetes, a public IP, or an inbound firewall rule.

## Network

Required outbound access:

- HTTPS to the configured Ahova service;
- HTTPS to GitHub Container Registry for installation and updates;
- local or private-network access to enabled model and storage services.

The local wizard listens on `127.0.0.1:7433`. Do not expose it publicly.

## Private AI

Install Ollama, LM Studio, or another compatible local server. Model requirements depend on the model,
quantization, context, and host, not Bridge itself.

As practical starting points:

- 8 GB system memory: small quantized models;
- 16 GB: many 7–8B quantized models;
- 32 GB or more: larger models or more comfortable concurrency;
- GPU acceleration is optional but can substantially reduce latency.

Choose a model that reliably understands the family's languages, produces structured JSON, and finishes
within bounded time. Test representative Ahova tasks before relying on it.

## Family storage

Prepare one of:

- a host folder, external disk, or mounted SMB/NFS share accessible to Docker;
- a dedicated WebDAV folder and account;
- a dedicated S3-compatible bucket or prefix and restricted access key.

Enable native provider snapshots or versioning. Bridge state backups do not contain family originals.

## Current preview limit

The current relay accepts files up to **5 MB each**. Ahova should keep larger unsupported originals in a
safe local queue until the configured product policy provides another supported route.

## Supported browsers

Use a current Safari, Chrome, Edge, or Firefox on the Bridge host. The local wizard supports English,
Russian, and Belarusian and follows the browser language by default.
