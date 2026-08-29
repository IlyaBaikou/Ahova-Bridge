# Troubleshooting

Bridge failures should be recoverable without exposing family content or granting remote shell access.

## Start here

From the installation folder run:

```bash
./ahova-bridge doctor
```

On Windows:

```powershell
.\ahova-bridge.ps1 doctor
```

Then open [http://127.0.0.1:7433](http://127.0.0.1:7433) and read the card that needs attention.

## Bridge page does not open

1. Confirm Docker is running.
2. Run `docker compose ps bridge`.
3. Check recent logs with `./ahova-bridge logs 100`.
4. Restart with `./ahova-bridge restart`.
5. Confirm another app is not using local port `7433`.

Do not solve this by exposing the setup port publicly.

## Pairing fails

- Confirm the code was created by a Family Owner.
- Use a newly generated code; it expires after ten minutes and works once.
- Verify outbound HTTPS, DNS, system time, and internet access.
- Confirm the Bridge is not already paired with another family.
- Unpair before changing the Ahova service address under **Advanced**.

A cancelled or failed code must not change the family's active storage.

## Private AI needs attention

- Start Ollama or the LM Studio server.
- Select **Find local AI** again.
- Pull or load at least one model.
- Use `host.docker.internal`, not `localhost`, for a service on the Docker host.
- On Linux, check the model server bind address and host firewall.
- Choose a smaller model if requests time out.

Bridge never silently falls back to paid managed AI.

## Mounted storage needs attention

- Verify the disk or NAS share is mounted on the host.
- Confirm Docker can access the selected folder.
- Run the `storage` helper again if the host path changed.
- Restore the original `.ahova-bridge-mount-id` with the correct storage.
- Never recreate the marker manually to force readiness.
- Check free capacity and read/write permissions.

## WebDAV needs attention

- Verify the exact folder URL and HTTPS certificate.
- Confirm the dedicated account and app password.
- Check read, write, delete, and folder-create permissions.
- Confirm the service is reachable from the Bridge host.

## S3 needs attention

- Check endpoint, bucket, region, and system time.
- Confirm path-style SigV4 compatibility.
- Verify the key is active and restricted to the configured bucket or prefix.
- Check quota, rate limiting, and bucket versioning policy.

## New files wait on the phone

Confirm that Personal storage is active and writable, then restore Bridge readiness. Existing records are
not deleted while the destination is unavailable. The phone may safely retain pending originals until
the configured policy allows upload again.

## Safe logs and diagnostics

- Recent logs: `./ahova-bridge logs 100`
- Safe diagnostic JSON: **Download safe diagnostics** in the local wizard

Diagnostics intentionally omit prompts, responses, model names, endpoints, paths, file names,
credentials, and tokens. Never send `.env`, state archives, passwords, access keys, pairing codes, or
family files to support.

## Report a reproducible problem

Include:

- Bridge version;
- host OS and architecture;
- Docker and Compose versions;
- enabled capability types without private endpoints;
- the safe error code or readiness state;
- safe diagnostics when requested.

Use [GitHub Issues](https://github.com/IlyaBaikou/Ahova-Bridge/issues) for non-sensitive reports. Follow
the repository security policy for vulnerabilities.
