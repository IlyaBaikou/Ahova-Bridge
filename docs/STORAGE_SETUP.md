# Connect family-owned storage

Ahova Bridge can store family originals in one selected destination: a mounted folder/NAS, WebDAV, or
S3-compatible object storage. Ahova keeps structured records, permissions, links, hashes, and safe opaque
locators; Bridge moves file bytes without exposing the storage credential to the phone.

The current preview relay accepts files up to 5 MB each. Selecting Bridge storage affects new originals.
Existing Ahova files remain where they are until an Owner starts and confirms an explicit migration.

## Before choosing a provider

- Create a dedicated family folder, WebDAV account, or bucket/prefix.
- Grant only the read/write/delete access Bridge needs for that destination.
- Enable snapshots, versioning, or backups on the storage platform itself.
- Confirm there is enough capacity and that the destination survives host restarts.
- Do not use the Bridge state backup as a backup of family originals.

## Mounted folder or NAS

This option works with a folder on the Docker host, an external disk, or an SMB/NFS share already mounted
by the host operating system.

1. Mount the disk or NAS share on the host and verify it is writable by Docker.
2. From the Bridge installation folder, choose it with the helper:

   ```bash
   ./ahova-bridge storage /mnt/family/ahova
   ```

   On Windows:

   ```powershell
   .\ahova-bridge.ps1 storage "D:\Family\Ahova"
   ```

   The helper validates or creates the folder, updates the private local setting, and safely recreates
   Bridge. On Docker Desktop, ensure the chosen folder is shared with Docker if prompted.

3. Open [http://127.0.0.1:7433](http://127.0.0.1:7433), enable **Use my family storage**, and choose
   **Mounted folder / NAS**.
4. Choose **Save and test**. A writable destination receives `.ahova-bridge-mount-id`.

Do not copy, edit, or delete the mount identity marker. It makes Bridge fail closed if a NAS mount
disappears and the host path unexpectedly points at a different disk. Mount the correct storage again
instead of recreating the marker manually.

Set `BRIDGE_STORAGE_READ_ONLY=true` only for an existing archive that Ahova should read without adding or
removing originals. Read-only storage cannot be selected as a writable destination for new files.

## WebDAV — Nextcloud or Synology

1. Create a dedicated least-privilege WebDAV user and family folder.
2. Prefer an HTTPS endpoint with a valid certificate.
3. In the Bridge wizard choose **WebDAV / Nextcloud** and enter:

   - the exact WebDAV endpoint for the family-owned folder;
   - the dedicated username;
   - the dedicated password or app password.

4. Choose **Save and test** and wait for the storage readiness card.

Bridge creates only the folders needed beneath the configured destination. Leaving the password field
empty during a later edit keeps the saved secret. The wizard never returns the saved password.

Plain HTTP is suitable only on a network you already trust or through a private tunnel. Do not expose a
WebDAV account or NAS administration surface solely to make Bridge reachable.

## S3-compatible storage

This option supports MinIO, Synology-compatible object storage, AWS S3, and compatible SigV4 services.

1. Create a dedicated bucket or prefix and an access key restricted to it.
2. Enable bucket versioning or snapshots where available.
3. In the Bridge wizard choose **S3 / MinIO** and enter:

   - **Storage endpoint:** the service endpoint, including HTTPS and any required port;
   - **S3 bucket:** the exact bucket name;
   - **S3 folder prefix:** a dedicated prefix such as `ahova`;
   - **S3 access key** and **S3 secret key:** the restricted credential;
   - **S3 region:** the service's signing region, often `us-east-1` for local MinIO.

4. Choose **Save and test** and wait for the storage readiness card.

Bridge uses SigV4 path-style requests so self-hosted endpoints work without wildcard DNS. Leaving a key
field empty during a later edit keeps the stored value. Never place bucket credentials in `.env`, logs,
screenshots, diagnostics, or support messages.

## Pair and select storage in Ahova

Once the storage card is healthy:

1. In Ahova, open **More → Files & Smart Actions → Ahova Bridge**.
2. Create a one-time code and pair the machine in the local wizard.
3. Return to **Files & Smart Actions** and confirm **Personal storage** is available.
4. Select it as the destination for new originals.
5. If existing Ahova files should move, start the explicit migration, review the source and target, and
   wait for copy-and-verify before cutover.

Pairing alone does not silently change the active family storage. Cancelling a code or closing the wizard
does not select Bridge. Existing files are not deleted or moved merely because another destination was
connected.

## Verify the full path

1. Confirm the local wizard reports storage **Ready**.
2. Run `./ahova-bridge doctor` or `.\ahova-bridge.ps1 doctor`.
3. Save one small non-sensitive test file from Ahova.
4. Confirm it appears beneath the chosen folder/prefix and remains openable through Ahova.
5. Remove the test file through Ahova and confirm the expected Trash/retention behavior rather than
   deleting provider data behind Ahova's back.

Do not use provider-side rename, overwrite, or deletion as the normal Ahova workflow. Ahova verifies
provider identity, revision, size, and hash; unexpected provider changes correctly surface as a recovery
state.

## Backup and recovery

Back up originals with the NAS, WebDAV, or S3 platform's native snapshot/versioning system. Separately
back up the small `bridge-state` volume because it contains the installation credential and crash-safe
job receipts:

```bash
./ahova-bridge backup
```

Store that archive encrypted. It is not safe to send to support and does not contain the mounted family
files. See [Operations and recovery](OPERATIONS.md) before restoring or moving an installation.

## Troubleshooting

- **Mounted storage unavailable:** verify the host mount and `.ahova-bridge-mount-id`; never recreate the
  marker manually.
- **WebDAV unavailable:** verify HTTPS, certificate trust, exact folder URL, app password, and account
  permissions.
- **S3 unavailable:** verify system time, endpoint, region, bucket, path-style compatibility, and the
  restricted key policy.
- **New files wait on the phone:** confirm Personal storage is active and writable, then restore Bridge
  readiness. Existing records are not deleted when storage is unavailable.
- **Migration pauses:** restore both source and target health, then use Ahova's resume or rollback action;
  do not manually copy partial provider folders.

Download safe diagnostics from the local wizard when needed. They intentionally omit endpoints, paths,
file names, credentials, and tokens.
