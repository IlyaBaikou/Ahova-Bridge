# FAQ

## Does Bridge replace the Ahova backend?

No. Ahova still manages family permissions, structured records, synchronization, validation, and action
confirmation. Bridge handles only the private AI and storage capabilities you enable.

## Do I need to open a router port?

No. Bridge makes outbound connections only. The setup page stays on localhost.

## Can I enable only private AI or only storage?

Yes. Both capabilities are independently switchable.

## Does private AI consume Smart Actions?

No. Successful work on your own model consumes zero Smart Actions. Ahova does not silently fall back to
paid managed processing when Bridge fails.

## Can family members chat freely with my local model?

No. Bridge is not a general chat proxy. It processes supported, bounded Ahova jobs and returns structured
proposals.

## Which local AI should I use?

Ollama and LM Studio have the simplest setup. Choose a recent instruction-tuned model that understands
your languages and reliably returns structured JSON.

## Why does Bridge use `host.docker.internal` instead of `localhost`?

Inside Docker, `localhost` means the Bridge container. `host.docker.internal` reaches a service running
on the Docker host.

## Where are WebDAV or S3 passwords stored?

In the protected local Bridge state volume. The wizard does not return saved secrets, and diagnostics
omit them.

## Can I store files on a NAS?

Yes. Mount the NAS on the host and select it with `./ahova-bridge storage /path/to/folder` or the Windows
equivalent.

## Does pairing automatically switch family storage?

No. An Owner must explicitly select Personal storage in Ahova.

## What happens to existing files when I connect storage?

Nothing automatically. Existing originals move only through an explicit copy-and-verify migration
confirmed by an Owner.

## What happens when Bridge or the NAS is offline?

Ahova keeps structured data intact and reports the capability as unavailable. Pending originals can stay
in a safe local queue according to product policy until storage returns.

## Is the Bridge backup also a backup of family files?

No. It contains credentials and operational state. Back up originals through the NAS, WebDAV, or S3
provider's snapshot or versioning tools.

## Can I run Bridge on a remote server?

Yes. Use SSH and a temporary localhost tunnel for setup. Normal work uses outbound HTTPS.

## Can I pair one Bridge with several families?

No. One installation pairs with one Ahova Family Workspace. Use separate installations and state volumes
for separate families.

## Can two machines run the same restored Bridge state?

No. Never run duplicated installation credentials concurrently. Move and verify one installation at a
time.

## How do I update Bridge?

Run `./ahova-bridge update`, then `./ahova-bridge doctor`. Use the `.ps1` helper on Windows.

## How do I remove Bridge?

Unpair it first, then run `./ahova-bridge uninstall`. Normal uninstall keeps state and family files.

## What can I safely send to support?

The safe diagnostic JSON, Bridge version, OS and Docker versions, and content-free error codes. Never send
credentials, `.env`, state backups, pairing codes, endpoints, paths, or family content.

## Is Bridge open source?

Yes. Source code and releases are available under the MIT License in the
[Ahova Bridge repository](https://github.com/IlyaBaikou/Ahova-Bridge).
