# Install Ahova Bridge

This guide takes a new machine from Docker to a paired and verified Ahova Bridge. The local setup page is
available only on the machine running Bridge unless you intentionally reach it through an SSH tunnel.
Do not expose it through a router or public reverse proxy.

## Before you begin

You need:

- Docker Desktop 4.30+ or Docker Engine 26+ with Compose v2, installed and running;
- a current macOS or Windows computer, or a 64-bit `amd64`/`arm64` Linux host;
- outbound HTTPS access to the Ahova service and GitHub Container Registry;
- an Ahova Family Owner account that can create a one-time pairing code;
- optional private AI and/or storage prepared using the linked capability guides.

Bridge itself needs about 100 MB plus a small state volume. Model files and family originals require
their own capacity. The current preview relay accepts files up to 5 MB each.

## macOS or Linux

Open Terminal in the parent folder where the Bridge installation should live, then run:

```bash
curl -fsSL https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.sh | sh
```

The installer resolves the latest stable GitHub release, creates `./ahova-bridge`, downloads its Compose
files, pulls the stable image, starts Bridge, and opens
[http://127.0.0.1:7433](http://127.0.0.1:7433).

To use another installation folder:

```bash
AHOVA_BRIDGE_INSTALL_DIR=/srv/ahova-bridge \
  sh -c 'curl -fsSL https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.sh | sh'
```

The target folder must either be empty or already contain a Bridge installation.

## Windows

Start Docker Desktop, open PowerShell in the parent folder where Bridge should live, and run:

```powershell
irm https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.ps1 | iex
```

The installer creates `ahova-bridge`, starts the container, and opens the local setup page. To use a
different folder:

```powershell
$env:AHOVA_BRIDGE_INSTALL_DIR = "D:\Ahova\Bridge"
irm https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.ps1 | iex
```

## Complete the local wizard

Open [http://127.0.0.1:7433](http://127.0.0.1:7433) and complete all four steps:

1. **Choose capabilities.** Enable private AI, family storage, or both.
2. **Connect services.** Select **Find local AI** to discover Ollama or LM Studio and choose a model.
   Choose a storage destination. Raw addresses and operational limits remain under **Advanced**.
3. **Test locally.** Select **Save and test**. Continue only after every enabled service is ready.
4. **Pair this machine.** In the Ahova app, open **More → Files & Smart Actions → Ahova Bridge**, create
   a one-time code, and paste it into the wizard. The code expires after ten minutes and works once.
5. **Verify readiness.** Wait until pairing, Ahova contact, and every enabled capability show healthy.

The wizard automatically uses English, Russian, or Belarusian from the browser. The language can also
be changed at the top of the page.

Cancelling or failing step 3 does not select Bridge as the family's storage. One Bridge installation is
paired with one Ahova Family Workspace. Revoke or unpair it before moving the state to another family.

## Verify from the command line

From the installation folder, run:

```bash
./ahova-bridge doctor
```

On Windows:

```powershell
.\ahova-bridge.ps1 doctor
```

A ready installation confirms:

1. Docker and Compose configuration;
2. a healthy Bridge process;
3. pairing and recent Ahova contact;
4. every enabled AI or storage capability.

You can also open `http://127.0.0.1:7433/health/live` for process health and
`http://127.0.0.1:7433/health/ready` for complete readiness.

## Finish setup in Ahova

After Bridge is ready:

- private AI becomes available to the family and consumes zero Smart Actions;
- Bridge storage appears as **Personal storage** under **More → Files & Smart Actions**;
- selecting Personal storage changes the destination for new originals only;
- existing originals move only after an Owner starts and confirms an explicit migration.

## Remote Linux server

Run the macOS/Linux installer over SSH on the server. The wizard remains bound to the server's loopback
interface. On your own computer, create a temporary tunnel:

```bash
ssh -L 7433:127.0.0.1:7433 user@your-server
```

Keep that terminal open, then visit [http://127.0.0.1:7433](http://127.0.0.1:7433) locally and complete
the wizard. Close the tunnel when setup is finished. Normal Bridge work uses outbound HTTPS and does not
need the tunnel.

## Choose a release channel

The one-line installer resolves the newest stable release from GitHub, and the default image follows
the stable `latest` tag. To deliberately pin a release, set `AHOVA_BRIDGE_VERSION` before installation
and set the corresponding container image in `.env`, for example:

```dotenv
AHOVA_BRIDGE_IMAGE=ghcr.io/ilyabaikou/ahova-bridge:0.1.0
```

Then recreate the container:

```bash
docker compose pull
docker compose up -d
./ahova-bridge doctor
```

Review release notes before changing major versions. Release archives include a SHA-256 checksum; the
published container supports Linux `amd64` and `arm64`.

## Update, back up, or remove

The installed helper keeps routine operations in one place:

```bash
./ahova-bridge update
./ahova-bridge backup
./ahova-bridge logs
./ahova-bridge uninstall
```

Use the `.ps1` helper on Windows. Uninstall keeps state and family files by default. Before purging local
state, disconnect Bridge in **More → Files & Smart Actions → Ahova Bridge** or choose **Unpair this
machine** in the wizard. The destructive `--purge-local-state` option requires typing `PURGE` and still
does not remove the mounted family folder.

See [Operations and recovery](OPERATIONS.md) for backup, restore, rollback, and incident handling.

## If setup does not become ready

1. Run `./ahova-bridge doctor` or `.\ahova-bridge.ps1 doctor`.
2. Open the local wizard and read the capability card that needs attention.
3. Check recent logs with `./ahova-bridge logs 100`.
4. Download the safe diagnostic JSON from the wizard if you need support.

Diagnostics intentionally omit prompts, responses, model names, endpoints, paths, file names,
credentials, and tokens. Never send the `.env`, state backup, NAS contents, or credentials to support.
