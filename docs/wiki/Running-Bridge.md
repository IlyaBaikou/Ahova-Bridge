# Running Bridge

Bridge is designed to require little routine attention. Use the helper installed beside `compose.yaml`.

## Everyday commands

| macOS / Linux | Windows | Purpose |
| --- | --- | --- |
| `./ahova-bridge open` | `.\ahova-bridge.ps1 open` | open the local setup and status page |
| `./ahova-bridge doctor` | `.\ahova-bridge.ps1 doctor` | verify configuration and readiness |
| `./ahova-bridge update` | `.\ahova-bridge.ps1 update` | pull the configured image and recreate Bridge |
| `./ahova-bridge backup` | `.\ahova-bridge.ps1 backup` | back up credentials and crash-safe state |
| `./ahova-bridge logs 100` | `.\ahova-bridge.ps1 logs 100` | show recent content-redacted logs |
| `./ahova-bridge restart` | `.\ahova-bridge.ps1 restart` | restart the container |
| `./ahova-bridge uninstall` | `.\ahova-bridge.ps1 uninstall` | remove the container but keep state and files |

## Health model

- `/health/live` proves the process answers locally.
- `/health/ready` additionally requires pairing, recent Ahova contact, and healthy enabled capabilities.
- The local wizard gives the most actionable readiness explanation.
- `./ahova-bridge doctor` checks Docker, Compose, process health, and readiness together.

First-time setup and a temporarily unavailable model or NAS correctly make readiness unavailable.

## Update

Review the [latest release](https://github.com/IlyaBaikou/Ahova-Bridge/releases/latest), then run:

```bash
./ahova-bridge update
./ahova-bridge doctor
```

The installer and `latest` image follow stable releases. Preview and release-candidate tags never replace
`latest`. A long-running server may pin a semantic version in `.env`.

## Back up Bridge state

```bash
./ahova-bridge backup
```

The helper briefly stops Bridge and creates a protected archive. It contains the installation credential
and job receipts, so store it encrypted with access restricted to the family operator.

This archive does not back up mounted, WebDAV, or S3 originals. Back those up with the provider's native
snapshot or versioning tools.

## Restore or move to another machine

1. Stop Bridge on the old machine.
2. Back up state and the storage destination separately.
3. Prepare an empty replacement state volume on the trusted target machine.
4. Restore the state archive while Bridge is stopped.
5. Mount or reconnect the exact family storage with its identity marker.
6. Start Bridge and run `doctor`.
7. Verify recent contact in Ahova before retiring the old host.

Never run two copies of the same restored installation concurrently. If Ahova reports that it was
revoked, discard the restored credential and pair a new installation.

## Change the mounted family folder

```bash
./ahova-bridge storage /new/family/folder
```

On Windows use `.\ahova-bridge.ps1 storage "D:\New\Family\Folder"`. Return to the wizard and select
**Save and test** before switching the active destination in Ahova.

## Unpair or uninstall

Unpair in the local wizard or Ahova before moving Bridge to another family. Normal uninstall keeps the
state volume and family files:

```bash
./ahova-bridge uninstall
```

`uninstall --purge-local-state` removes the credential volume only after explicit `PURGE` confirmation.
It still keeps the mounted family folder. Purge only after revoking Bridge in Ahova.

## Release rollback

Pin the previous semantic image tag, recreate Bridge, and run `doctor`. Do not roll back state unless the
release notes explicitly require it. Never cross a major protocol version without its migration guide.
