# Operations and recovery

Ahova Bridge makes only outbound connections. Do not expose the wizard, local model, or mounted NAS
service to the public internet as part of an operations workaround.

## Health model

- `/health/live` proves the process can answer locally. It is suitable for container restart policy.
- `/health/ready` additionally requires pairing, a working control-plane connection, and healthy
  enabled capabilities. It is unavailable during first-time setup and after the worker observes a
  connection failure, including revoked access. `/api/v1/setup/verify` uses the same readiness rule.
  `lastControlPlaneContactAt` is historical evidence, not proof of a current connection;
  `controlPlaneConnected` is restored only by a successful worker request. A rejected local job
  does not itself disconnect the control plane. Readiness is observed at worker requests, so a
  revocation is reflected after the next request, not at the instant the owner revokes access.
- `/api/v1/diagnostics` is loopback-only and content-redacted. Capture it before restart when practical.

## Change the mounted family folder

Do not edit Compose or `.env` by hand for the normal path. From the installation folder run:

```bash
./ahova-bridge storage /mnt/family/ahova
```

On Windows use `.\ahova-bridge.ps1 storage "D:\Family\Ahova"`. The helper recreates only the Bridge
container; it does not copy, move, or delete existing family originals. Return to the local wizard and
select **Save and test** before switching the active destination in Ahova.

## Backup

Back up the family storage mount, WebDAV service, or S3 bucket with the storage platform's snapshot or
versioning facility. Separately
back up the small Docker state volume while Bridge is stopped:

```bash
docker compose stop bridge
docker run --rm -v ahova-bridge_bridge-state:/state:ro -v "$PWD":/backup alpine \
  tar -czf /backup/ahova-bridge-state.tgz -C /state .
docker compose start bridge
```

The archive contains a credential. Store it encrypted with access restricted to the family operator.
Do not attach it to support requests.

## Restore

Restore onto a trusted host with Bridge stopped. Use an empty replacement volume, restore the archive,
start Bridge, and verify the local wizard and readiness endpoint. Also restore or mount the exact family
storage volume with its `.ahova-bridge-mount-id` marker. A missing or changed marker must fail closed.

If Ahova reports that the installation was revoked, discard the restored Bridge state and pair a new
installation. Do not restore an old credential over a newer active installation.

## Common incidents

- `control-plane-unavailable`: verify outbound HTTPS, system time, DNS, and the configured Ahova URL.
- AI unavailable: verify the local endpoint from the Docker host and confirm the exact model is in the
  allowlist. Bridge never falls back to paid managed AI.
- Storage unavailable: for mounted storage, verify the host mount and identity marker before restarting.
  Never recreate the marker manually to force a mount healthy. For WebDAV or S3, verify TLS, system time,
  the dedicated credential, bucket/folder permission, endpoint health, and capacity without copying a
  production secret into diagnostics.
- Pending receipt count grows: keep the state volume, restore control-plane access, and allow Bridge to
  replay completion safely. Receipts older than the configured retention period are removed because the
  corresponding bounded lease is no longer executable.
- Disk full: stop Bridge, free capacity outside the mounted family content, verify the mount, then start
  again. Do not delete credential or receipt files selectively.

## Upgrade and rollback

Use a stable semantic-version tag or immutable digest. Back up state, pull the target image, recreate
the container, then check readiness. Roll back the image without rolling back state unless the release
notes explicitly require it. Never cross a major protocol version without its migration guide.
