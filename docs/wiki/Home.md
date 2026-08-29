<p align="center">
  <img src="https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/src/Ahova.Bridge/wwwroot/assets/ahova-logo.png" width="96" alt="Ahova Bridge">
</p>

# Ahova Bridge

**Your private AI and family-owned storage, connected to Ahova.**

Ahova Bridge is a small open-source service that runs on your computer, home server, or NAS. It lets
Ahova use capabilities you operate yourself without opening your home network to the internet.

- **Private AI:** process supported Ahova requests with Ollama, LM Studio, or another compatible model.
- **Family-owned storage:** keep originals in a local folder, NAS, WebDAV service, or S3 bucket.
- **Outbound only:** no public IP, inbound tunnel, or router changes.
- **Family controlled:** pair one Bridge with one Ahova Family Workspace and revoke it at any time.

## Choose your path

| I want to… | Start here |
| --- | --- |
| install Bridge for the first time | [Quick Start](Quick-Start) |
| use my own model without Smart Actions | [Private AI](Private-AI) |
| keep originals on my hardware or cloud account | [Family Storage](Family-Storage) |
| update, back up, move, or remove Bridge | [Running Bridge](Running-Bridge) |
| fix a setup or readiness problem | [Troubleshooting](Troubleshooting) |
| understand the security boundary | [Privacy & Security](Privacy-and-Security) |
| check hardware, software, and network requirements | [System Requirements](System-Requirements) |
| find a short answer | [FAQ](FAQ) |

## Five-minute start

Install Docker Desktop or Docker Engine with Compose v2, then run the installer.

**macOS or Linux**

```bash
curl -fsSL https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.sh | sh
```

**Windows PowerShell**

```powershell
irm https://raw.githubusercontent.com/IlyaBaikou/Ahova-Bridge/main/scripts/install.ps1 | iex
```

The installer resolves the latest stable release, starts Bridge, and opens the local setup page at
[http://127.0.0.1:7433](http://127.0.0.1:7433). The wizard supports English, Russian, and Belarusian.

## What Bridge is — and is not

Bridge handles only the private capabilities you enable. Ahova continues to handle family permissions,
structured records, validation, synchronization, and confirmation before an action is saved.

Bridge is not a self-hosted copy of the Ahova backend, a general chat proxy, a public NAS gateway, or a
remote administration tunnel. Private model results remain proposals and pass the same rules as managed
processing.

## Get help safely

Start with [Troubleshooting](Troubleshooting) and download **safe diagnostics** from the local wizard.
Diagnostics omit prompts, responses, model names, endpoints, paths, file names, credentials, and tokens.
Never send your `.env`, state backup, NAS contents, passwords, or access keys to support.

Source code, releases, and issue tracking live in the
[Ahova Bridge repository](https://github.com/IlyaBaikou/Ahova-Bridge).
