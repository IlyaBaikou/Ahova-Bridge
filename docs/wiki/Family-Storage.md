# Family Storage

Bridge can keep family originals in one selected destination. Ahova retains structured records,
permissions, links, hashes, and opaque locators; Bridge moves file bytes without exposing the storage
credential to the phone.

## Choose a provider

| Destination | Best for | Bridge setup |
| --- | --- | --- |
| Folder or NAS | home server, external disk, mounted SMB/NFS | one local helper command |
| WebDAV | Nextcloud, Synology WebDAV, compatible cloud | endpoint, username, app password |
| S3-compatible | MinIO, Synology-compatible storage, AWS S3 | endpoint, bucket, restricted key |

Before connecting storage:

- create a dedicated family folder, account, bucket, or prefix;
- grant only the access Bridge needs;
- enable native snapshots, versioning, or backups;
- verify the destination survives host restarts;
- confirm Docker can access it.

## Folder on this machine or NAS

Mount an external disk or NAS share on the host first. From the Bridge installation folder run:

```bash
./ahova-bridge storage /mnt/family/ahova
```

On Windows:

```powershell
.\ahova-bridge.ps1 storage "D:\Family\Ahova"
```

The helper validates or creates the folder, updates the private local setting, and recreates Bridge. It
does not copy, move, or delete existing family files.

Then open the wizard, enable family storage, choose **Folder on this machine or NAS**, and select
**Save and test**. A writable destination receives `.ahova-bridge-mount-id`.

Do not edit or delete the marker. Bridge uses it to fail closed if a NAS disappears and the host path
unexpectedly points to another disk.

## WebDAV / Nextcloud / Synology

1. Create a dedicated least-privilege WebDAV user and family folder.
2. Prefer HTTPS with a valid certificate.
3. In Bridge choose **WebDAV / Nextcloud**.
4. Enter the exact folder endpoint, username, and password or app password.
5. Select **Save and test**.

Leaving the password empty during a later edit keeps the saved secret. The wizard never returns it.

## S3 / MinIO

1. Create a dedicated bucket or prefix.
2. Create a key restricted to that destination.
3. Enable bucket versioning where available.
4. In Bridge choose **S3 / MinIO** and enter:
   - service endpoint;
   - bucket;
   - prefix such as `ahova`;
   - access and secret keys;
   - signing region, often `us-east-1` for local MinIO.
5. Select **Save and test**.

Bridge uses SigV4 path-style requests for self-hosted endpoints. Never place bucket credentials in logs,
screenshots, diagnostics, or support messages.

## Select storage in Ahova

Pairing Bridge does not silently change active family storage. After the storage card is healthy:

1. open **More → Files & Smart Actions → Ahova Bridge**;
2. confirm **Personal storage** is available;
3. select it for new originals;
4. save a small non-sensitive test file and open it again through Ahova.

## Existing files and migration

Existing originals stay where they are until an Owner starts an explicit migration. Ahova should:

1. copy to the target;
2. verify size and hash;
3. wait for owner confirmation;
4. cut over only after verification;
5. retain a recovery path according to the source storage policy.

If migration pauses, restore both source and target health and use Ahova's resume or rollback action. Do
not manually copy a partially migrated provider folder.

## Backups

Use the storage platform's native snapshot or versioning system for originals. Separately back up the
small Bridge state volume:

```bash
./ahova-bridge backup
```

The Bridge state archive contains credentials and receipts, not family originals. Store it encrypted and
never send it to support. See [Running Bridge](Running-Bridge).
