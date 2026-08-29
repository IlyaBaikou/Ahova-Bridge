# Contributing

Keep changes narrow and reviewable. Every protocol, credential, filesystem, networking, logging, update,
or AI-output change requires tests and a threat-model review.

Before opening a change:

```bash
dotnet format Ahova.Bridge.slnx --verify-no-changes
dotnet test Ahova.Bridge.slnx
docker build .
```

Use synthetic fixtures only. Never commit a real control-plane URL, pairing code, installation secret,
token, prompt, model output, mount path, or household file.

## Wiki documentation

The public Wiki is generated from `docs/wiki/*.md`. Update and review those files with the related code,
then publish them from an authenticated maintainer checkout:

```bash
./scripts/publish-wiki.sh
```

Do not edit the public Wiki independently: the next source-based publication intentionally replaces its
Markdown pages. The publication script copies only top-level Wiki Markdown and never reads or publishes
runtime configuration, diagnostics, or family content.
