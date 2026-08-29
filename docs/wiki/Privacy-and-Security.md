# Privacy & Security

Ahova Bridge is a security-sensitive, user-operated capability worker. Its architecture minimizes the
network and data surface rather than asking the family to trust a remote administration layer.

## Network boundary

- Bridge makes outbound connections to Ahova.
- The setup page binds to `127.0.0.1` only.
- No public IP, router port, inbound tunnel, or Kubernetes ingress is required.
- A remote server should use a temporary SSH tunnel for setup.
- Local AI and NAS services should remain on a trusted host or private network.

## Capability boundary

Private AI and storage are independently switchable. Bridge receives typed, bounded jobs rather than an
arbitrary command or shell interface.

- Model results are untrusted proposals.
- Ahova validates structured output and authorization before saving an action.
- Storage paths are resolved inside the configured root and reject traversal and links.
- File relays are size bounded and checksum verified.
- Leases expire, cancellation is honored, and completed jobs use crash-safe replay protection.

## Credentials

- Pairing uses a short-lived, one-time code.
- The installation credential stays in the protected Bridge state volume.
- WebDAV and S3 secrets are never returned by the wizard.
- Unpairing revokes the installation remotely when Ahova is available.
- Forced local removal clearly requires later remote revocation.

Treat a Bridge state backup like a password. Store it encrypted and never attach it to support requests.

## Mounted storage identity

Bridge records a random marker in the configured mounted root and protected state. If a NAS disappears
and the same path points to another disk, Bridge fails closed instead of writing family data there.

Do not copy, edit, or recreate `.ahova-bridge-mount-id` manually.

## Diagnostics and logs

Safe diagnostics contain operational state only. They omit:

- family prompts and responses;
- model names and endpoints;
- storage endpoints and paths;
- file names;
- credentials, tokens, and pairing codes.

Content logging must not be enabled as a troubleshooting shortcut.

## Host hardening

The released container:

- runs as a non-root user;
- drops Linux capabilities;
- uses a read-only root filesystem;
- enables `no-new-privileges`;
- limits processes and temporary storage;
- persists only explicit state and family-storage mounts.

Also keep the host OS, Docker, model server, and storage platform updated. Restrict local access to the
Bridge installation folder and state backups.

## Public verification

Source code, Docker configuration, adapters, protocol boundaries, tests, security policy, and threat
model are available in the
[repository](https://github.com/IlyaBaikou/Ahova-Bridge).

Read the current [Security Policy](https://github.com/IlyaBaikou/Ahova-Bridge/blob/main/SECURITY.md) and
[Threat Model](https://github.com/IlyaBaikou/Ahova-Bridge/blob/main/THREAT_MODEL.md) before deploying on a
new network.
