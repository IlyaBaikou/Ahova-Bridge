# Quick Start

This guide takes a new machine from Docker to a paired and verified Ahova Bridge.

## 1. Before you begin

You need:

- Docker Desktop 4.30+ on macOS or Windows, or Docker Engine 26+ with Compose v2 on Linux;
- a current 64-bit `arm64` or `amd64` machine;
- outbound HTTPS access to Ahova and GitHub Container Registry;
- an Ahova Family Owner account;
- at least one capability: private AI, family storage, or both.

See [System Requirements](System-Requirements) for model and storage guidance.

## 2. Install Bridge

Open Terminal or PowerShell in the parent folder where the installation should live.

**macOS or Linux**

```bash
curl -fsSL https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.sh | sh
```

**Windows PowerShell**

```powershell
irm https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.ps1 | iex
```

The installer:

1. verifies Docker and Compose;
2. resolves the newest stable Ahova Bridge release;
3. creates an `ahova-bridge` folder;
4. starts the container;
5. opens [http://127.0.0.1:7433](http://127.0.0.1:7433).

The setup page is available only on the Bridge machine. Do not publish port `7433` through a router or
public reverse proxy.

## 3. Complete the wizard

1. **Capabilities:** enable private AI, family storage, or both.
2. **Services:** let Bridge find Ollama or LM Studio and choose a model. Choose a storage destination if
   needed. Technical fields stay under **Advanced**.
3. **Test:** select **Save and test**. Continue only after every enabled service is ready.
4. **Pair:** in Ahova open **More → Files & Smart Actions → Ahova Bridge**, create a one-time code, and
   enter it in the local wizard.
5. **Ready:** wait until Ahova contact and every enabled capability are healthy.

The pairing code expires after ten minutes and works once. A cancelled or failed pairing does not change
the family's active storage.

## 4. Verify from the installation folder

```bash
./ahova-bridge doctor
```

On Windows:

```powershell
.\ahova-bridge.ps1 doctor
```

A ready installation confirms Docker, the Bridge process, pairing, recent Ahova contact, private AI,
and storage.

## 5. Finish in Ahova

- Private AI becomes available for supported work and consumes zero Smart Actions.
- Bridge storage appears as **Personal storage**.
- Selecting Personal storage changes the destination for new originals only.
- Existing originals move only after an Owner starts and confirms an explicit copy-and-verify migration.

## Remote Linux server

Run the installer over SSH. Keep the setup page private with a temporary tunnel:

```bash
ssh -L 7433:127.0.0.1:7433 user@your-server
```

Open `http://127.0.0.1:7433` on your computer, finish setup, then close the tunnel. Normal Bridge work
uses outbound HTTPS and does not need the tunnel.

## Next steps

- [Set up Private AI](Private-AI)
- [Choose Family Storage](Family-Storage)
- [Learn routine operations](Running-Bridge)
